using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SpaceRace.Messages
{
    public class Messenger
    {
        Dictionary<Type, List<RecipientAndMethod>> _registered = new Dictionary<Type, List<RecipientAndMethod>>();
 
        private void Messanger()
        {
        }

        #region Properties

        private static Messenger _instance;

        public static Messenger Instance
        {
            get
            {
                if(_instance == null)
                    _instance = new Messenger();
                return _instance;
            }
        }

        #endregion

        #region Public Methods

        public void Send<TMessage>(TMessage msg)
        {
            Type type = typeof (TMessage);

            if (!_registered.ContainsKey(type))
                return;

            var list = _registered[type];

            foreach (var recipientAndMethod in list)
            {
                recipientAndMethod.MethodInfo.Invoke(recipientAndMethod.Recipient, new object[] {msg});
            }
        }

        public void Register<TMessage>(object recipient, Action<TMessage> action)
        {
            Type type = typeof (TMessage);

            List<RecipientAndMethod> list = (_registered.ContainsKey(type)) ? _registered[type] : new List<RecipientAndMethod>();

            RecipientAndMethod rm = new RecipientAndMethod()
            {
                MethodInfo = action.Method,
                Recipient =  recipient
            };
            list.Add(rm);

            if(!_registered.ContainsKey(type))
                _registered.Add(type, list);
        }

        #endregion

        #region Private Struct

        private struct RecipientAndMethod
        {
            public MethodInfo MethodInfo { get; set; }
            public object Recipient { get; set; }
        }

        #endregion

    }
}
