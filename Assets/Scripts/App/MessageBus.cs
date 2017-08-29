using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CX.CamTool
{
    public partial class MessageBus: Core.Singleton.SingletonApplicationLifetimeLazy< MessageBus >
    {
        public System.Action<string, bool, bool> addToConsole;
        public void SendConsoleMessage(string s, bool scrollTo, bool forceShow)
        {
            if (addToConsole != null)
            {
                addToConsole(s, scrollTo, forceShow);
            }
        }
        public void clear()
        {
            addToConsole = null;
        }

        /*
        public System.Action<Type> exampleAction;
        public void sendExampleAction(Type t)
        {
            if (exampleAction != null)
            {
                exampleAction(t);
            }
            else // (optional)
            {
                Debug.LogWarning("No exampleAction");
            }
        }
        */
    }

}
