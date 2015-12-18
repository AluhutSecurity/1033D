using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1033D {
    class Program {
        static uint MYUID = 22347;
        static ushort sequenceCount;
        
        static void Main( string[] args ) {

            var arduino = new _1033C.Communication.MyArduinoHandler();
            Console.WriteLine( "please select a port: (by index)" );
            int i = 0;
            foreach(var portname in arduino.PortNames) {
                Console.WriteLine( "{0}: {1}", i++, portname );
            }
            var cline = Console.ReadLine();
            arduino.PortName = arduino.PortNames[int.Parse( cline )];
            Console.WriteLine( "selected port: {0}", arduino.PortName );
            arduino.Open();
            Console.ReadKey();
            arduino.Close();

            var client = BlakcMakiga.Abrakadubra.CreateInstance( BlakcMakiga.BlakcInstanceIdentifier.BlakcClient ) as BlakcMakiga.Net.IBlakcClient;
            client.Connect( new System.Net.IPEndPoint( _1033C.Defines.DEFAULT_DRONESERVER_IP, _1033C.Defines.DEFAULT_DRONESERVER_LOCALPORT ) );

            bool loginresult = client.Login( MYUID );
            if ( loginresult == false ) {
                Console.WriteLine( "login rejected" );
                Console.ReadKey();
                return;
            }
            Console.WriteLine( "login successful" );
            sequenceCount = 0;

            var packageGenerator = new _1033C.Drones.MyDronePacketGenerator( BlakcMakiga.Utils.CRC.CRCMode.CRC16 );
            _1033C.Drones.MyDronePacket packet;
            Random r = new Random();

            for ( i = 0; i < 200; i++ ) {
                System.Threading.Thread.Sleep( 1000 );
                byte[] blob = ( BitConverter.GetBytes( r.NextDouble() * 100 ).Concat( BitConverter.GetBytes( r.NextDouble() * 100 ) ) ).Concat( BitConverter.GetBytes( r.NextDouble() * 20 ) ).ToArray();
                packet = packageGenerator.GeneratePacket( _1033C.Drones.MyDronePacketContent.LocationUpdate, blob );
                client.Send( sequenceCount++, packet.Serialize() );
            }

            packet = packageGenerator.GeneratePacket( _1033C.Drones.MyDronePacketContent.SignalBye );
            client.Send( ++sequenceCount, packet.Serialize() );

            client.Stop();
            Console.WriteLine( "disconnected" );
            Console.ReadKey();
        }
    }
}
