namespace Lab4
{
    /**
        Name: Paul Hwang
        Date: October 19th, 2022
        Description: Lab 4 - Using our previous lab submission and creating unit tests for it to ensure
                             that our functions work properly (both happy and unhappy paths)
                             
        Bugs:       Not really a bug but more of thing that works but maybe shouldn't be done in practice. I realized that SetUp()
                    is being ran after each test. I thought that it would only run only once at beginning. So bl = new BusinessLogic();
                    is being ran over and over when it should really be ran once. I tried to take it out of the SetUp() and have it just
                    exist at the top where bl is being declared but that didn't seem to work       
        
        Reflection: Setting up the testing environment was a disaster on Mac. I tried to run parallels but it kept giving me
                    errors so I ended up just doing the lab on my desktop. After I switched over to my desktop, the process was
                    pretty linear and it ended up working just fine. The only issue I have is that we're testing against the actual
                    database rather than mocking it out. I originally had a "TestDatabase" class that mocked out the functionality of the
                    RelationalDatabase but was informed to just test against the actual bit.io database. It resulted in me having to
                    rework my code a bit but all my tests have passed and seem to be working.
    */

    public static class MauiProgram
    {
        public static IBusinessLogic ibl = new BusinessLogic();

        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            return builder.Build();
        }
    }

}
