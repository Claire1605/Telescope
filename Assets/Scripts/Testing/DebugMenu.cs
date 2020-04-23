using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DebugStatic
{
    public static bool testIntro = false;

    public static bool AreWeTestingIntro()
    {
        bool testing = false;
#if UNITY_EDITOR
        if (testIntro)
        {
            testing = true;

        }
#endif
        return testing;
    }
}
