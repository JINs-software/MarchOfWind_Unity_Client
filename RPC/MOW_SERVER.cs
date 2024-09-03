
using System;

public class MOW_SERVER : Stub_MOW_SERVER
{
    private void Start() 
    {
        base.Init();
    }

    private void OnDestroy()
    {
        base.Clear();  
    }


}
