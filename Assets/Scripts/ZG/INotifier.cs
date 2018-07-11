using System;
using System.Collections.Generic;
using System.Text;


public interface INotifier
{
    void OnReceiveData(uint cmdId, object param1, object param2);
}

