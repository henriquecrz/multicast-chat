using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace client
{
    class Client
    {
        private IPAddress enderecoIP;

        private TcpClient tcpServidor;

        private bool Conectado;

        private StreamWriter stwEnviador;
        private StreamReader strReceptor;

        private delegate void AtualizaLogCallBack(string strMensagem);

        private Thread mensagemThread;

        public void Start()
        {
            try
            {
                AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnApplicationExit);

                // Console.Write("Informe o IP da sala em que deseja conversar:");
                // var ipAddress = Console.ReadLine();

                Console.WriteLine("digite seu usuário");
                var usuario = Console.ReadLine();

                // Trata o endereço IP informado em um objeto IPAdress
                enderecoIP = IPAddress.Parse("127.0.0.1");
                // Inicia uma nova conexão TCP com o servidor chat
                tcpServidor = new TcpClient();
                tcpServidor.Connect(enderecoIP, 2502);

                // AJuda a verificar se estamos conectados ou não
                Conectado = true;

                // Envia o nome do usuário ao servidor
                stwEnviador = new StreamWriter(tcpServidor.GetStream());
                stwEnviador.WriteLine(usuario);
                stwEnviador.Flush();

                //Inicia a thread para receber mensagens e nova comunicação
                mensagemThread = new Thread(new ThreadStart(RecebeMensagens));
                mensagemThread.Start();

                Console.WriteLine("Chat:");

                while (true)
                {
                    EnviaMensagem();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro : " + ex.Message, "Erro na conexão com servidor");
            }
        }

        // Envia a mensagem para o servidor
        private void EnviaMensagem()
        {
            var message = Console.ReadLine();

            if (message == "/quit")
            {
                FechaConexao("Você desconectou do chat");
            }
            else
            {
                stwEnviador.WriteLine(message);
                stwEnviador.Flush();
            }
        }

        private void RecebeMensagens()
        {
            // recebe a resposta do servidor
            strReceptor = new StreamReader(tcpServidor.GetStream());

            // Enquanto estiver conectado le as linhas que estão chegando do servidor
            while (Conectado)
            {
                Console.WriteLine(strReceptor.ReadLine());
            }
        }

        // Fecha a conexão com o servidor
        private void FechaConexao(string message)
        {
            // Mostra o motivo porque a conexão encerrou
            Console.WriteLine(message);

            // Fecha os objetos
            Conectado = false;
            stwEnviador.Close();
            strReceptor.Close();
            tcpServidor.Close();
        }

        // O tratador de evento para a saida da aplicação
        public void OnApplicationExit(object sender, EventArgs e)
        {
            if (Conectado == true)
            {
                // Fecha as conexões, streams, etc...
                Conectado = false;
                stwEnviador.Close();
                strReceptor.Close();
                tcpServidor.Close();
            }
        }
    }
}
