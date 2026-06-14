using HarmonyLib;
using Il2CppRUMBLE.Input;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using Il2CppTMPro;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using Object = UnityEngine.Object;

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
    private GameObject? _cubeMesh;
    
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        CurrentScene = sceneName;
        
        if (sceneName != "Loader")
        {
            
            MelonLogger.Msg("Loaded Keyboard api");
            // BuildKeyboard(new Vector3(2.0f, 1.5f, 0.0f), Quaternion.Euler(-45, 0, 0), KeyPressed, true);
        }
    }

    public override void OnFixedUpdate()
    {
        
    }

    private String _written = "";

    public void KeyPressed(string key)
    {
        if (key == "Enter")
        {
            MelonLogger.Msg(_written);
            _written = "";
        }
        else
        {
            _written += key;
        }
    }

    public GameObject BuildKeyboard(Vector3 pos, Quaternion rot, Action<string> onKeyPressed, bool following)
    {
        if (_cubeMesh == null)
        {
            _cubeMesh = Il2CppRUMBLE.Managers.PoolManager.instance.GetAllStructurePrefabsFromPool()[5].processableComponent.gameObject.transform.GetChild(0).gameObject;
        }
        
        GameObject keyboard = new GameObject();
        
        keyboard.name = "Keyboard";
        keyboard.transform.position = pos;
        keyboard.transform.rotation = rot;
        keyboard.AddComponent<Keyboard>().Parent = keyboard;
        keyboard.GetComponent<Keyboard>().Following = following;
        
        /*
        var outText = Create.NewText();
        outText.transform.parent = Keyboard.transform;
        outText.transform.localPosition = new Vector3(0.0f, 2.0f, 0.0f);
        outText.transform.localRotation = Quaternion.identity;
        outText.transform.GetComponent<TextMeshPro>().text = "Test";
        outText.name = "Text";
        */

        var keys = "0;Q;W;E;R;T;Z;U;I;O;P;A;S;D;F;G;H;J;K;L; ;Y;X;C;V;B;N;M; ; ; ; ;".Split(";");

        var keyboardKeys = "1,^ 1,1 1,2 1,3 1,4 1,5 1,6 1,7 1,8 1,9 1,0 1,+ 3,Del:/2,Tab s, 1,q 1,w 1,e 1,r 1,t 1,z 1,u 1,i 1,o 1,p 32,Enter:/s, s, 1,a 1,s 1,d 1,f 1,g 1,h 1,j 1,k 1,l:/3,Shift s, s, 1,y 1,x 1,c 3,Space s, s, 1,v 1,b 1,n 1,m 2,Paste";
        var keyboardKeys_shift = "1,^ 1,! 1,€ 1,§ 1,$ 1,% 1,& 1,/ 1,( 1,) 1,= 1,* 3,Del:/2,Tab s, 1,Q 1,W 1,W 1,R 1,T 1,Z 1,U 1,I 1,O 1,P 32,Enter:/s, s, 1,A 1,S 1,D 1,F 1,G 1,H 1,J 1,K 1,L:/3,Shift s, s, 1,Y 1,X 1,C 3,Space s, s, 1,V 1,B 1,N 1,M 2,Paste";
        var keysize = 0.12f;
        
        List<Vector2> taken_pos = null;

        var x = 0;
        var y = 0;
        
        var rows = keyboardKeys.Split(":/");
        Vector3 offset = new Vector3(rows[0].Split(" ").Length*-0.14f/2f, 0.0f, 0.0f);
        
        for (int i = 0; i < rows.Length; i++)
        {
            var row_keys = rows[i].Split(" ");
            for (int j = 0; j < row_keys.Length; j++)
            {
                var new_key = row_keys[j];
                var name = "";

                if (new_key[0] != 's')
                {
                    Vector2 size = new Vector2(int.Parse(new_key[0].ToString()), 1);
                    if (new_key[1] != ',')
                    {
                        size.y = int.Parse(new_key[1].ToString());
                        name = row_keys[j].Substring(3);
                    }

                    else
                    {
                        name = row_keys[j].Substring(2);
                    }

                    MelonLogger.Msg("Lenght: " + size);
                    MelonLogger.Msg("Key: " + name);

                    CreateNewBigButton(new Vector3(0.14f * x, 0.0f, -0.14f * y) + offset, Quaternion.identity, name, size, 0.12f, keyboard, onKeyPressed, onKeyPressed, onKeyPressed);
                }

                x++;
            }

            y++;
            x = 0;
        }
        
        /*
        Vector3 offset = new Vector3(0.12f * 5f, 0.0f, -0.12f * 2f);
        
        for (int x = 0; x < 10; x++)
        {
            for (int z = 0; z < 3; z++)
            {
                String letter = keys[x+1+(z*10)];

                if (letter != " ")
                {
                    CreateNewButton(new Vector3(0.12f * x, 0.0f, -0.12f * z) - offset, Quaternion.identity, letter, keyboard, onKeyPressed);
                }
            }
        }
        
        CreateNewBigButton(new Vector3(0.12f * 7, 0.0f, -0.12f * 2) - offset, Quaternion.identity, "Enter", 3, 0.12f, keyboard, onKeyPressed);
        
        */

        return keyboard;
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
                
        var newCube = Object.Instantiate(_cubeMesh, gameObject.transform);
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

    private void CreateNewBigButton(Vector3 position, Quaternion rotation, String letter, Vector2 scale, float size, GameObject keyboard, Action<string> onKeyPressed, Action<string> onKeyUp, Action<string> onKeyDown, Vector3? textSize = null)
    {
        Vector3 finalTextSize = textSize ?? new Vector3(4f, 10f, 10f);
        List<GameObject> gameobjects = new List<GameObject>();
        
        var gameObject = new GameObject();
        gameObject.transform.parent = keyboard.transform;
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.name = letter;
        gameObject.AddComponent<Keyboard_Button>().Letter = letter;
        gameObject.GetComponent<Keyboard_Button>().OnButtonPressed += onKeyPressed;
        gameObject.GetComponent<Keyboard_Button>().OnButtonUp += onKeyUp;
        gameObject.GetComponent<Keyboard_Button>().OnButtonDown += onKeyDown;
        gameobjects.Add(gameObject);
            
        var newCube = Object.Instantiate(_cubeMesh, gameObject.transform);
        newCube.transform.localPosition = new Vector3((scale.x/2 -0.5f) * size, 0.0f, -(scale.y/2 -0.5f) * size);
        newCube.transform.localRotation = Quaternion.identity;
        
        Vector2 scaledScale = scale * (size+0.02f);
        scaledScale = new Vector2(scaledScale.x - 0.02f, scaledScale.y - 0.02f);
        
        newCube.transform.localScale = new Vector3(scaledScale.x, 0.1f, scaledScale.y);
        newCube.name = "Button";
            
        var text = Create.NewText();
        text.transform.parent = newCube.transform;
        text.transform.localPosition = new Vector3(0.0f, 0.62f, 0.0f);
        text.transform.localRotation = Quaternion.Euler(90, 0, 0);
        text.transform.localScale = finalTextSize;
        text.transform.GetComponent<TextMeshPro>().text = letter;
        
        position = new Vector3(position.x + size, position.y + size, position.z);
        
        for (int y = 0; y < scale.y; y++)
        {
            for (int x = 0; x < scale.x; x++)
            {
                gameObject = new GameObject();
                gameObject.transform.parent = keyboard.transform;
                gameObject.transform.localPosition = new Vector3((float)(position.x + x*(size+0.02)), position.y, -(float)(position.z + y*(size+0.02)));
                gameObject.transform.localRotation = rotation;
                gameObject.name = letter;
            
                gameobjects.Add(gameObject);
            }
        }
        
        gameobjects[0].GetComponent<Keyboard_Button>().buttonParts = gameobjects.ToArray();
        gameobjects[0].GetComponent<Keyboard_Button>().Default_Position = new Vector3((scale.x/2 -0.5f) * size, 0.0f, -(scale.y/2 -0.5f) * size);
        
        /*
        Vector3 finalTextSize = textSize ?? new Vector3(4f, 10f, 10f);
        List<GameObject> gameobjects = new List<GameObject>();
        
        var gameObject = new GameObject();
        gameObject.transform.parent = keyboard.transform;
        gameObject.transform.localPosition = position;
        gameObject.transform.localRotation = rotation;
        gameObject.name = letter;
        gameObject.AddComponent<Big_Button>().Letter = letter;
        gameObject.GetComponent<Big_Button>().OnPressed += onKeyPressed;
        gameobjects.Add(gameObject);
            
        var newCube = Object.Instantiate(_cubeMesh, gameObject.transform);
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
        */
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
    
        Vector3 rightHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.righthand.Index.BoneC.position;
        Vector3 leftHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.lefthand.Index.BoneC.position;
        
        Vector3 pos = Parent.transform.position;
        Quaternion rot = Parent.transform.rotation;

        float size = 0.02f;
        Vector3 normal = rot * Vector3.up;
        
        Vector3 pos_point = pos + (normal * 0.14f);
        pos += normal * size;
        
        float distRight = Vector3.Magnitude(rightHandPos - pos_point);
        float distLeft = Vector3.Magnitude(leftHandPos - pos_point);
        
        if (distLeft < 0.1f)
        {
            Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.lHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
        }
        else
        {
            Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.lHandInput = null;
        }
        if (distRight < 0.1f)
        {
            Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.rHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
        }
        else
        {
            Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.rHandInput = null;
        }
        
        distRight = Vector3.Magnitude(rightHandPos - pos);
        distLeft = Vector3.Magnitude(leftHandPos - pos);
    
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
internal class Keyboard_Button : MonoBehaviour
{
    public String Letter = "OO";
    public event Action<string>? OnButtonPressed;
    public event Action<string>? OnButtonDown;
    public event Action<string>? OnButtonUp;
    
    public GameObject[]? buttonParts;
    public Boolean pressed = false;
    
    public Vector3 Default_Position = new Vector3(0.0f, 0.0f, 0.0f);
    
    public void FixedUpdate()
    {
        var player = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer();
        if (player == null) return;
        if (buttonParts == null) return;

        if (player.Controller?.PlayerScaling?.rigDefinition == null) return;
        
    
        Vector3 rightHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.righthand.Index.BoneC.position;
        Vector3 leftHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.lefthand.Index.BoneC.position;

        var is_pressed = false;
        
        foreach (GameObject parent in buttonParts)
        {
            Vector3 pos = parent.transform.position;
            Quaternion rot = parent.transform.rotation;

            float size = 0.02f;
            Vector3 normal = rot * Vector3.up;
            
            Vector3 pos_point = pos + (normal * 0.14f);
            pos += normal * size;
        
            float distRight = Vector3.Magnitude(rightHandPos - pos_point);
            float distLeft = Vector3.Magnitude(leftHandPos - pos_point);
        
            if (distLeft < 0.1f)
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.lHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.lHandInput = null;
            }
            if (distRight < 0.1f)
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.rHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.rHandInput = null;
            }
            
            distRight = Vector3.Magnitude(rightHandPos - pos);
            distLeft = Vector3.Magnitude(leftHandPos - pos);

            float dist = Math.Min(distRight, distLeft);

            if (dist < 0.08f)
            {
                is_pressed = true;
            }
        }

        if (is_pressed)
        {
            pressed = true;
            buttonParts[0].transform.GetChild(0).transform.localPosition = Vector3.Lerp(Default_Position, new Vector3(Default_Position.x, -0.1f, Default_Position.z), 2);
            
            if (OnButtonDown != null) OnButtonDown?.Invoke(Letter);
        }
        else if (pressed)
        {
            pressed = false;
            buttonParts[0].transform.GetChild(0).transform.localPosition = Default_Position;
            
            if (OnButtonUp != null) OnButtonUp?.Invoke(Letter);
            
            if (OnButtonPressed != null) OnButtonPressed?.Invoke(Letter);
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
        
    
        Vector3 rightHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.righthand.Index.BoneC.position;
        Vector3 leftHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.lefthand.Index.BoneC.position;

        var is_pressed = false;
        
        foreach (GameObject parent in Parents)
        {
            Vector3 pos = parent.transform.position;
            Quaternion rot = parent.transform.rotation;

            float size = 0.02f;
            Vector3 normal = rot * Vector3.up;
            
            Vector3 pos_point = pos + (normal * 0.14f);
            pos += normal * size;
        
            float distRight = Vector3.Magnitude(rightHandPos - pos_point);
            float distLeft = Vector3.Magnitude(leftHandPos - pos_point);
        
            if (distLeft < 0.1f)
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.lHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.lHandInput = null;
            }
            if (distRight < 0.1f)
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.rHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.rHandInput = null;
            }
            
            distRight = Vector3.Magnitude(rightHandPos - pos);
            distLeft = Vector3.Magnitude(leftHandPos - pos);

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

[RegisterTypeInIl2Cpp]
internal class Keyboard : MonoBehaviour
{
    public bool Following = false;
    public float distance = 0.35f;
    public GameObject? Parent;
    public float player_height = 1.2f;
    
    public float positionSmoothTime = 0.2f;
    public float rotationSmoothSpeed = 5f;
    
    private Vector3 velocity = Vector3.zero;
    private Quaternion? lastTargetRotation;
    
    public void Update()
    {
        var player = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer();
        if (player == null) return;
        if (Parent == null) return;
        if (lastTargetRotation == null) lastTargetRotation = Parent.transform.rotation;

        if (Following)
        {
            
            var cam = Camera.main;
            if (cam == null) return;
            
            Vector3 forward = cam.transform.forward;
            forward.y = 0f;
            forward.Normalize();

            Vector3 bodyPos = player.Controller.PlayerPhysics.transform.position;
            Vector3 newPos = bodyPos + forward * distance;

            Vector3 dir = newPos - cam.transform.position;
            dir.y = 0f;

            if (dir != Vector3.zero)
            {
                Quaternion desiredRotation = Quaternion.LookRotation(dir) * Quaternion.Euler(-45f, 0f, 0f);
                
                Parent.transform.rotation = Quaternion.Slerp(
                    Parent.transform.rotation,
                    desiredRotation,
                    Time.deltaTime * rotationSmoothSpeed
                );
            }
            
            newPos.y += player_height;
            
            float angleDifference = Quaternion.Angle((Quaternion)lastTargetRotation, Parent.transform.rotation);
            
            float dynamicSmoothTime = positionSmoothTime;
            
            if (angleDifference > 30f)
            {
                dynamicSmoothTime = 0.05f;
            }
            
            Parent.transform.position = Vector3.SmoothDamp(
                Parent.transform.position,
                newPos,
                ref velocity,
                dynamicSmoothTime
            );

            lastTargetRotation = Parent.transform.rotation;
        }
    }
}

[HarmonyPatch(typeof(PlayerHandPresence), nameof(PlayerHandPresence.UpdateHandPresenceAnimationStates))]
public class Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates
{
    public static PlayerHandPresence.HandPresenceInput? lHandInput;
    public static PlayerHandPresence.HandPresenceInput? rHandInput;
    
    static void Prefix(PlayerHandPresence __instance, InputManager.Hand hand, ref PlayerHandPresence.HandPresenceInput input)
    {
        if (__instance.parentController == null) return;
        
        if (__instance.parentController.ControllerType != ControllerType.Local) return;
        
        if (hand == InputManager.Hand.Left && lHandInput is { } l)
            input = l;

        if (hand == InputManager.Hand.Right && rHandInput is { } r)
            input = r;
    }
}
