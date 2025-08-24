using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace YagizEraslan.Claude.Unity
{
    public static class UnityWebRequestAwaiter
    {
        public static TaskAwaiter<UnityWebRequest> GetAwaiter(this UnityWebRequestAsyncOperation request)
        {
            var tcs = new TaskCompletionSource<UnityWebRequest>();
            
            // Use a local variable to prevent closure capture of the TaskCompletionSource
            System.Action<UnityEngine.AsyncOperation> completionHandler = null;
            completionHandler = _ =>
            {
                // Remove the handler to prevent memory leaks
                request.completed -= completionHandler;
                tcs.SetResult(request.webRequest);
            };
            
            request.completed += completionHandler;
            return tcs.Task.GetAwaiter();
        }
    }
}
