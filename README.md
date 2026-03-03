This is an API that adds a Keyboard

EXAMPLE USAGE:

public string CurrentScene = "Loader";
public GameObject? CubeMesh;

public override void OnSceneWasLoaded(int buildIndex, string sceneName)
{
    CurrentScene = sceneName;

    if (sceneName != "Loader")
    { 
        BuildKeyboard(new Vector3(2.0f, 1.5f, 0.0f), Quaternion.Euler(-45, 0, 0), onKeyPressed);
    }
}

public String written = "";

public void onKeyPressed(string key)
{
    if (key == "Enter")
    {
        MelonLogger.Msg(written);
        written = "";
    }
    else
    {
        written += key;
    }
}

In a Mod

BuildKeyboard will also return a gameobject so reference that for moving.



Things I hopefully will add:
    . Typewriter like text
    . Player following
    . Casing
    . Custom Casing and Buttons

(Note: these are about in the order I will prob do them.)