using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _1033D {
    class Program {
        static uint MYUID = 22347;
        static ushort sequenceCount;

        private static void Handler() {

        }

        static void Main( string[] args ) {

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

            for(int i = 0; i < 20; i++) {                
                System.Threading.Thread.Sleep( 1000 );
                byte[] blob = BitConverter.GetBytes( r.NextDouble() * 100 ).Concat( BitConverter.GetBytes( r.NextDouble() * 100 ) ).Concat( BitConverter.GetBytes( r.NextDouble() * 20 ) ).ToArray();
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
