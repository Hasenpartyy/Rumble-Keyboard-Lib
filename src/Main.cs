using Il2CppRUMBLE.Interactions.InteractionBase;
using Il2CppTMPro;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UnityEngine.Playables;
using Object = UnityEngine.Object;
using Stream = Il2CppSystem.IO.Stream;

[assembly: MelonInfo(typeof(YT_Mod.Main), YT_Mod.BuildInfo.Name, YT_Mod.BuildInfo.Version, YT_Mod.BuildInfo.Author)]
[assembly: MelonGame("Buckethead Entertainment", "RUMBLE")]
[assembly: MelonAdditionalDependencies("RumbleModdingAPI")]

namespace YT_Mod;

public static class BuildInfo
{
    public const string Name = "YT_Mod";
    public const string Author = "Hasenparty";
    public const string Version = "0.0.1";
    public const string FormatVersion = "1.0.0";
}

public class Main : MelonMod
{
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

    public GameObject BuildKeyboard(Vector3 pos, Quaternion rot, Action<string> onKeyPressed)
    {
        if (CubeMesh == null)
        {
            CubeMesh = Il2CppRUMBLE.Managers.PoolManager.instance.GetAllStructurePrefabsFromPool()[5].processableComponent.gameObject.transform.GetChild(0).gameObject;
        }
        
        GameObject Keyboard = new GameObject();
        
        Keyboard.name = "Keyboard";
        Keyboard.transform.position = pos;
        Keyboard.transform.rotation = rot;
        
        /*
        var outText = Create.NewText();
        outText.transform.parent = Keyboard.transform;
        outText.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
        outText.transform.localRotation = Quaternion.identity;
        outText.transform.GetComponent<TextMeshPro>().text = "Test";
        outText.name = "Text";
        */

        var keys = "0;Q;W;E;R;T;Z;U;I;O;P;A;S;D;F;G;H;J;K;L; ;Y;X;C;V;B;N;M; ; ; ; ;".Split(";");

        String letter = "";
        
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                letter = keys[x+1+(z*10)];

                if (letter != " ")
                {
                    CreateNewButton(new Vector3(0.12f * x, 0.0f, -0.12f * z), Quaternion.identity, letter, Keyboard, onKeyPressed);
                }
            }
        }
        
        CreateNewBigButton(new Vector3(0.12f * 7, 0.0f, -0.12f * 2), Quaternion.identity, "Enter", 3, 0.12f, Keyboard, onKeyPressed);

        return Keyboard;
    }

    private void CreateNewButton(Vector3 position, Quaternion rotation, String letter, GameObject keyboard, Action<string> onKeyPressed)
    {
        var gameObject = new GameObject();
        gameObject.transform.parent = keyboard.transform;
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.name = letter;
        gameObject.AddComponent<Button>().Parent = gameObject;
        gameObject.GetComponent<Button>().Letter = letter;
        gameObject.GetComponent<Button>().OnPressed += onKeyPressed;
                
        var newCube = Object.Instantiate(CubeMesh, gameObject.transform);
        newCube.transform.localPosition = Vector3.zero;
        newCube.transform.localRotation = Quaternion.identity;
        newCube.transform.localScale = Vector3.one / 10f;
        newCube.name = "Button";
                
        var text = Create.NewText();
        text.transform.parent = newCube.transform;
        text.transform.localPosition = new Vector3(0.0f, 0.62f, 0.0f);
        text.transform.localRotation = Quaternion.Euler(90, 0, 0);
        text.transform.localScale = new Vector3(4.0f, 10.0f, 10.0f);
        text.transform.GetComponent<TextMeshPro>().text = letter;
    }

    private void CreateNewBigButton(Vector3 position, Quaternion rotation, String letter, int cnt, float size, GameObject keyboard, Action<string> onKeyPressed, Vector3? text_size = null)
    {
        Vector3 finalTextSize = text_size ?? new Vector3(4f, 10f, 10f);
        List<GameObject> gameobjects = new List<GameObject>();
        
        var gameObject = new GameObject();
        gameObject.transform.parent = keyboard.transform;
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.name = letter;
        gameObject.AddComponent<Big_Button>().Letter = letter;
        gameObject.GetComponent<Big_Button>().OnPressed += onKeyPressed;
        gameobjects.Add(gameObject);
            
        var newCube = Object.Instantiate(CubeMesh, gameObject.transform);
        newCube.transform.localPosition = new Vector3(((float)cnt/2 -0.5f) * 0.12f, 0.0f, 0.0f);
        newCube.transform.localRotation = Quaternion.identity;
        float scale = (cnt - 1) * size + 0.1f;
        newCube.transform.localScale = new Vector3(scale, 0.1f, 0.1f);
        newCube.name = "Button";
            
        var text = Create.NewText();
        text.transform.parent = newCube.transform;
        text.transform.localPosition = new Vector3(0.0f, 0.62f, 0.0f);
        text.transform.localRotation = Quaternion.Euler(90, 0, 0); 
        text.transform.localScale = finalTextSize;
        text.transform.GetComponent<TextMeshPro>().text = letter;
        
        position = new Vector3(position.x + size, position.y, position.z);
        
        for (int x = 0; x < cnt-1; x++)
        {
            gameObject = new GameObject();
            gameObject.transform.parent = keyboard.transform;
            gameObject.transform.localPosition = new Vector3(position.x + x*size, 0.0f, position.z);
            gameObject.transform.localRotation = rotation;
            gameObject.name = letter;
            
            gameobjects.Add(gameObject);
        }
        
        gameobjects[0].GetComponent<Big_Button>().Parents = gameobjects.ToArray();
        gameobjects[0].GetComponent<Big_Button>().Default_Position = new Vector3(((float)cnt/2 -0.5f) * 0.12f, 0.0f, 0.0f);
    }
}

[RegisterTypeInIl2Cpp]
internal class Button : MonoBehaviour
{
    public String Letter = "OO";
    public GameObject Parent;
    public Boolean Pressed = false;
    public event Action<string> OnPressed;

    public void FixedUpdate()
    {
        var player = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer();
        if (player == null) return;
        if (Parent == null) return;

        if (player.Controller?.PlayerScaling?.rigDefinition == null) return;
        
    
        Vector3 rightHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerScaling
            .rigDefinition.RightHandDefinition.Transform.position;
    
        Vector3 leftHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerScaling
            .rigDefinition.LeftHandDefinition.Transform.position;
        
        Vector3 pos = new Vector3(Parent.transform.position.x, Parent.transform.position.y + 0.04f, Parent.transform.position.z);
    
        float distRight = Vector3.Magnitude(rightHandPos - pos);
        float distLeft = Vector3.Magnitude(leftHandPos - pos);
    
        float dist = Math.Min(distRight, distLeft);
        
        if (dist < 0.08f)
        {
            Pressed = true;
            
            Parent.transform.GetChild(0).transform.localPosition = Vector3.Lerp(new Vector3(0.0f, 0.0f, 0.0f), new Vector3(0.0f, -0.1f, 0.0f), 2);
        }
        else if (Pressed)
        {
            Pressed = false;
            Parent.transform.GetChild(0).transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            
            OnPressed?.Invoke(Letter);
            // Parent.transform.parent.GetChild(0).GetComponent<TextMeshPro>().text = Parent.transform.parent.GetChild(0).GetComponent<TextMeshPro>().text + Letter;
        }
    }
}

[RegisterTypeInIl2Cpp]
internal class Big_Button : MonoBehaviour
{
    public String Letter = "OO";
    public GameObject[] Parents;
    public Boolean Pressed = false;
    public Vector3 Default_Position = new Vector3(0.0f, 0.0f, 0.0f);
    public event Action<string> OnPressed;

    public void FixedUpdate()
    {
        var player = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer();
        if (player == null) return;
        if (Parents == null) return;

        if (player.Controller?.PlayerScaling?.rigDefinition == null) return;
        
    
        Vector3 rightHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerScaling
            .rigDefinition.RightHandDefinition.Transform.position;
    
        Vector3 leftHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerScaling
            .rigDefinition.LeftHandDefinition.Transform.position;

        var is_pressed = false;

        Vector3 pos = new Vector3(Parents[0].transform.position.x, Parents[0].transform.position.y + 0.04f, Parents[0].transform.position.z);
        
        foreach (GameObject parent in Parents)
        {
            float distRight = Vector3.Magnitude(rightHandPos - pos);
            float distLeft = Vector3.Magnitude(leftHandPos - pos);

            float dist = Math.Min(distRight, distLeft);

            if (dist < 0.08f)
            {
                is_pressed = true;
            }
        }

        if (is_pressed)
        {
            Pressed = true;
            Parents[0].transform.GetChild(0).transform.localPosition = Vector3.Lerp(Default_Position, new Vector3(Default_Position.x, -0.1f, Default_Position.z), 2);
        }
        else if (Pressed)
        {
            Pressed = false;
            Parents[0].transform.GetChild(0).transform.localPosition = Default_Position;
            OnPressed?.Invoke(Letter);
            // Parents[0].transform.parent.GetChild(0).GetComponent<TextMeshPro>().text = Parents[0].transform.parent.GetChild(0).GetComponent<TextMeshPro>().text + Letter;
        }
    }
}
