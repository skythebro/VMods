using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using Bloodstone.API;
using VMods.Shared;

namespace VMods.ResourceStashWithdrawal
{
    public class ResourceStashWithDrawalRequester
    {
        private static Queue<ResourceStashWithdrawalRequest> requests = new();
        private static bool running = false;
        static int fps = 30;
        static int frameDelay = 1000 / fps;

        public static void StartTask(ResourceStashWithdrawalRequest request)
        {
            try
            {
                lock (requests)
                {
                    requests.Enqueue(request);

                    if (!running)
                    {
                        running = true;
                        RunActionsAsync();
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Logger.LogError($"Error while trying to start a task: {e.Message} Stacktrace: {e.StackTrace}");
            }
        }

        private static async void RunActionsAsync()
        {
            try
            {
                while (requests.Count > 0)
                {
                    ResourceStashWithdrawalRequest nextRequest;

                    lock (requests)
                    {
                        nextRequest = requests.Dequeue();
                    }

                    VNetwork.SendToServerStruct(nextRequest);


                    // Add a delay cuz otherwise the server will get flooded with requests, I could run coroutine but this works too
                    await Task.Delay(frameDelay);
                }

                running = false;
            }
            catch (Exception e)
            {
                Utils.Logger.LogError($"Error while trying to run a task async: {e.Message} Stacktrace: {e.StackTrace}");
            }
        }
    }
}