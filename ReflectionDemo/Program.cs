using System;
using BlackBoxDemo;


namespace myProgram
{
    class Program
    {
        // Note zero references to methods and variables
        public string myData = "This is the public string MyData";
        private string myOtherPrivateData = "This is the PRIVATE string myOtherPrivateData";

        private void myProtectedData()
        {
            // note this method is never actually called from the main app here, so it "should" never execute
            String mySecret = "Secret #1";
            Console.WriteLine("This is a PRIVATE method myProtectedData() that prints myOtherPrivateData = " + myOtherPrivateData);
            // Console.WriteLine("Info: = " + mySecret.Length);
        }

        public void myPublicData()
        {
            // note this method is never actually called from the main app here, so it "should" never execute
            String mySecret = "Secret #2";
            Console.WriteLine("This is a public method myPublicData() that prints myData = " + myData);
            // Console.WriteLine("Info: = " + mySecret.Length);
        }

        static void Main(string[] args)
        {
            int LocalSecretInteger = 42;
            string LocalSecretString = " Secret String" + LocalSecretInteger.ToString();
            Console.WriteLine(LocalSecretString.Substring(1, 1));

            // simply instantiate the blackbox demo to allow it
            // acccess to our local, private methods and variables!
            BlackBox bb = new BlackBoxDemo.BlackBox();

            Console.WriteLine("Done!");
        }
    }

}