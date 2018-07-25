using System;
using BlackBox;


namespace myProgram
{
    class Program
    {
        // Note zero references to methods and variables
        public string myData = "This is the public string MyData";
        private string myOtherPrivateData = "This is the PRIVATE string myOtherPrivateData";

        private void MyPrivateData()
        {
            // note this method is never actually called from the main app here, so it "should" never execute
            String mySecret = "Secret #1";
            Console.WriteLine("This is a PRIVATE method myPrivatedData() that prints myOtherPrivateData = " + myOtherPrivateData);
            Console.WriteLine(mySecret.Substring(0,0));
        }

        public void MyPublicData()
        {
            // note this method is never actually called from the main app here, so it "should" never execute
            String mySecret = "Secret #2";
            Console.WriteLine("This is a public method myPublicData() that prints myData = " + myData);
            Console.WriteLine(mySecret.Substring(0, 0));
        }

        static void Main(string[] args)
        {
            int LocalSecretInteger = 42;
            string LocalSecretString = " Secret String" + LocalSecretInteger.ToString();
            Console.WriteLine(LocalSecretString.Substring(0, 0)); // we don't actually print our secret

            // simply instantiate the blackbox demo to allow it
            // acccess to our local, private methods and variables!
            BlackBoxPeeker bb = new BlackBox.BlackBoxPeeker();

            Console.WriteLine("Done!");
            // Console.ReadLine();
        }
    }

}