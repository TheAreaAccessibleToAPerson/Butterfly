namespace Butterfly
{
    public class Program
    {
        static void Main(string[] args)
            => Gudron.run<Header2>(new Gudron.Settings()
            {
                Name = "",
                StartingTime = 1000,
                SystemEvent = new EventSetting("", 10, 256000),

                Events = new EventSetting[]
                {
                    new EventSetting("1", 5, 256000),
                    new EventSetting("2", 5, 1256000),
                    new EventSetting("3", 50, 1256000),
                    new EventSetting("4", 50, 1256000)
                }
            });
    }

    public sealed class Header : Controller
    {
        void Start()
        {
            obj<Header2>("HEADER2");
        }
    }


    public sealed class Header2 : Controller
    {
        void Construction()
        {
            listen_echo<string>("echotest")
                .output_to((message, response) => 
                {
                    Console(message);

                    response.To("ответ");
                });
            
            listen_message<string>("messagetest")
                .output_to((message) => 
                {
                    Console(message);
                });

            int i = 0;
            add_event("1", () => 
            {
                if (i > 1000) return;

                obj<Test2>("Test2" + i++);
                obj<Test2>("Test2" + i++);
            });
        }

        void Start()
        {
            Console("START");
        }

        void Configurate()
        {
        }
        void Stop()
        {
        }
    }

    public sealed class Test2 : Controller.Board
    {
        IInput<string> send_echo, send_message;

        void Construction()
        {
            send_echo<string>(ref send_echo, "echotest")
                .output_to((message) => 
                {
                    Console($"Ответ:" + message);
                });    

            send_message<string>(ref send_message, "messagetest");

            obj<Test3>("TEST3");
            obj<Test3>("TEST31");
            obj<Test3>("TEST32");
            obj<Test3>("TEST33");
            obj<Test3>("TEST34");
            obj<Test3>("TEST35");
            obj<Test3>("TEST36");
            obj<Test3>("TEST37");

            add_event("4", Update);
        }

        void Update()
        {
        }

        void Start()
        {
            send_echo.To("Hello1");
            send_message.To("Hello2");
        }

        void Stop()
        {
        }

        void Configurate()
        {
        }

    }

    public sealed class Test3 : Controller
    {
        void Construction()
        {
            obj<Test4>("TEST444");
            obj<Test4>("TEST433");
            obj<Test4>("TEST422");
            obj<Test4>("TEST411");
            obj<Test4>("TEST49");
            obj<Test4>("TEST48");
            obj<Test4>("TEST47");
            obj<Test4>("TEST46");
            obj<Test4>("TEST45");
            obj<Test4>("TEST43");
            obj<Test4>("TEST42");
            obj<Test4>("TEST41");

            add_event("3", Update);
        }

        void Update()
        {

        }

        void Start()
        {
            //Console("START");
        }

        void Configurate()
        {
            //destroy();
        }
    }

    public sealed class Test4 : Controller
    {
        void Construction()
        {
            //destroy();
            add_event("4", Update1);
            add_event("1", Update1);
            add_event("2", Update1);
            add_event("3", Update1);
        }

        void Update1()
        {
            //destroy();
        }

        void Update()
        {
        }

        void Start()
        {
            destroy();
            //Console("START");
        }

        void Configurate()
        {
        }
    }
}
