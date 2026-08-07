using HarmonyLib;
using Il2CppRUMBLE.Input;
using Il2CppRUMBLE.Players;
using Il2CppRUMBLE.Players.Subsystems;
using MelonLoader;
using MelonLoader.Utils;
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
        
        string defaultPath = Path.Combine(MelonEnvironment.UserDataDirectory, "Keyboard_API");
        if (!File.Exists(defaultPath))
        {
            Directory.CreateDirectory(defaultPath);
            MelonLogger.Msg("Created Folder for Layouts.");
        }
        
        if (sceneName != "Loader")
        {
            MelonLogger.Msg("Loaded Keyboard api");
            BuildKeyboardFromJson(new Vector3(2.0f, 1.2f, 0.0f), Quaternion.Euler(325, 180, 180), KeyPressed, KeyPressed, KeyPressed, false);
            // BuildKeyboard(new Vector3(2.0f, 1.5f, 0.0f), Quaternion.Euler(-45, 0, 0), KeyPressed, true);
        }
    }

    public override void OnFixedUpdate()
    {
        
    }

    private String _written = "";

    private void KeyPressed(string key)
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
    
    public GameObject BuildKeyboardFromJson(Vector3 pos, Quaternion rot, Action<string> onKeyPressed, Action<string> onKeyUp, Action<string> onKeyDown, bool following)
    {
        if (_cubeMesh == null)
        {
            _cubeMesh = Il2CppRUMBLE.Managers.PoolManager.instance.GetAllStructurePrefabsFromPool()[5].processableComponent.gameObject.transform.GetChild(0).gameObject;
        }
        
        // Get JSON file path
        string filePath = Path.Combine(MelonEnvironment.UserDataDirectory, "Keyboard_API", "au-layout.json");
        
        if (!File.Exists(filePath))
        {
            MelonLogger.Error($"Layout file not found at: {filePath}");
            return null;
        }
        
        // Convert to raw JSON
        string rawJson = File.ReadAllText(filePath);
        
        // MelonLogger.Msg(rawJson);

        // Parse Into better readable data
        List<List<KeyData>> layout = KleParser.ParseKleJson(rawJson);
        
        if (layout == null || layout.Count == 0)
        {
            MelonLogger.Error("Parsed layout is empty! Check JSON formatting or parser.");
            return null;
        }

        MelonLogger.Msg($"Successfully parsed {layout.Count} rows.");
        
        // Keyboard Container
        GameObject keyboard = new GameObject("Keyboard") {
        transform =
        {
            position = pos,
            rotation = rot
        }};

        // Add the Component
        var keyboardComp = keyboard.AddComponent<Keyboard>();
        keyboardComp.parent = keyboard;
        keyboardComp.following = following;
            
        MelonLogger.Msg(layout);    

        // Spacing
        float keySpacing = 0.1f;

        // Generate Buttons
        for (int y = 0; y < layout.Count; y++)
        {
            var line = layout[y];
            float xOffset = 0.0f;
            for (int x = 0; x < line.Count; x++)
            {
                KeyData keyData = line[x];
                MelonLogger.Msg($"{keyData.Key} | {keyData.Size}");
                
                List<float> sizeList = new List<float>();

                foreach (string str in keyData.Size.Split("|"))
                {
                    sizeList.Add(float.Parse(str));
                }

                xOffset += sizeList[0]*keySpacing;
                Vector2 keyPos =  new Vector2(xOffset, y*keySpacing + sizeList[1]*keySpacing);
                Vector3 keySize = new Vector3(sizeList[2]*keySpacing, sizeList[3]*keySpacing, keySpacing);
                MelonLogger.Msg($"{keyPos} | {keySize}");

                if (sizeList.Count > 4)
                {
                    Vector2 keyPos2 =  new Vector2(keyPos.x + (sizeList[4]*keySpacing), keyPos.y + (sizeList[5]*keySpacing));
                    Vector3 keySize2 = new Vector3(sizeList[6]*keySpacing, sizeList[7]*keySpacing, keySpacing);
                    MelonLogger.Msg($"{keyPos2} | {keySize2}");
                    CreateButton(keyboard, onKeyPressed, onKeyUp, onKeyDown, keyData.Key, keyPos, keySize, keyPos2, keySize2);
                }
                else
                {
                    CreateButton(keyboard, onKeyPressed, onKeyUp, onKeyDown, keyData.Key, keyPos, keySize, Vector2.zero, Vector3.zero);
                }
                
                
                xOffset += sizeList[2]*keySpacing;
            }
        }

        return keyboard;
    }
    
    /*
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
        / *

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

        return keyboard;
    }*/

    private void CreateButton(GameObject keyboard, Action<string> onKeyPressed, Action<string> onKeyUp, Action<string> onKeyDown, String letter, Vector2 offset, Vector3 size, Vector2 offset2, Vector3 size2)
    {
        var gameObject = new GameObject { 
        transform =
        {
            parent = keyboard.transform,
            localPosition = Vector3.zero,
            localRotation = Quaternion.identity
        }, name = letter};
        gameObject.AddComponent<KeyboardButton>().letter = letter;
        gameObject.GetComponent<KeyboardButton>().OnButtonPressed += onKeyPressed;
        gameObject.GetComponent<KeyboardButton>().OnButtonUp += onKeyUp;
        gameObject.GetComponent<KeyboardButton>().OnButtonDown += onKeyDown;
        
        var firstCube = Object.Instantiate(_cubeMesh, gameObject.transform);
        firstCube.transform.localPosition = new Vector3(
            offset.x + (size.x / 2f),
            offset.y + (size.y / 2f),
            0f
        );
        firstCube.transform.localRotation = Quaternion.identity;
        firstCube.transform.localScale = size;
        firstCube.name = "Button1";

        if (size2.z != 0.0f)
        {
            var secondCube = Object.Instantiate(_cubeMesh, gameObject.transform);
            secondCube.transform.localPosition = new Vector3(
                offset2.x + (size2.x / 2f), 
                offset2.y + (size2.y / 2f), 
                0f
            );
            secondCube.transform.localRotation = Quaternion.identity;
            secondCube.transform.localScale = size2;
            secondCube.name = "Button2";
        }
    }
}

[RegisterTypeInIl2Cpp]
internal class KeyboardButton : MonoBehaviour
{
    public String letter = "OO";
    public event Action<string>? OnButtonPressed;
    public event Action<string>? OnButtonDown;
    public event Action<string>? OnButtonUp;
    
    public Boolean pressed;
    
    private bool IsPointInCube(Vector3 firstPos, Vector3 secondPos, Vector3 point)
    {
        // Find the minimum and maximum boundaries for each axis
        float minX = Mathf.Min(firstPos.x, secondPos.x);
        float maxX = Mathf.Max(firstPos.x, secondPos.x);

        float minY = Mathf.Min(firstPos.y, secondPos.y);
        float maxY = Mathf.Max(firstPos.y, secondPos.y);

        float minZ = Mathf.Min(firstPos.z, secondPos.z);
        float maxZ = Mathf.Max(firstPos.z, secondPos.z);

        // Check if the point lies within all three bounds
        return (point.x >= minX && point.x <= maxX) &&
               (point.y >= minY && point.y <= maxY) &&
               (point.z >= minZ && point.z <= maxZ);
    }
    
    public void FixedUpdate()
    {
        var player = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer();
        if (player == null) return;

        if (player.Controller?.PlayerScaling?.rigDefinition == null) return;
    
        Vector3 rightHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.righthand.Index.BoneC.position;
        Vector3 leftHandPos = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer().Controller.PlayerHandPresence.lefthand.Index.BoneC.position;
        
        int childCount = transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            Transform child = transform.GetChild(i);
            
            var firstPos = child.position - child.right*(child.localScale.x/2f) - child.up*(child.localScale.z/2f);
            var secondPos = child.position + child.right*(child.localScale.x/2f) + child.up*(child.localScale.z/2f) + child.forward*child.localScale.z + child.forward*(child.localScale.z*0.75f*Convert.ToInt32(pressed));

            float distRight = Vector3.Magnitude(rightHandPos - child.position);
            float distLeft = Vector3.Magnitude(leftHandPos - child.position);
        
            if (distLeft < 0.2f)
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.LHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.LHandInput = null;
            }
            if (distRight < 0.2f)
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.RHandInput = new PlayerHandPresence.HandPresenceInput(0.0f, 1.0f, 1.0f, 0.0f);
            }
            else
            {
                Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates.RHandInput = null;
            }

            if (IsPointInCube(firstPos, secondPos, leftHandPos) || IsPointInCube(firstPos, secondPos, rightHandPos))
            {
                pressed = true;
                
                if (OnButtonDown != null) OnButtonDown?.Invoke(letter);
            }
            else if (pressed)
            {
                pressed = false;
                
                if (OnButtonUp != null) OnButtonUp?.Invoke(letter);
            }
        }

        if (pressed)
        {
            transform.localPosition = new Vector3(0, 0, transform.GetChild(0).localScale.z*-0.75f);
            if (OnButtonPressed != null) OnButtonPressed?.Invoke(letter);
        }
        else
        {
            transform.localPosition = new Vector3(0, 0, 0);
        }
        
        /*
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
        }*/
    }
}

[RegisterTypeInIl2Cpp]
internal class Keyboard : MonoBehaviour
{
    public bool following = false;
    public float distance = 0.35f;
    public GameObject? parent;
    public float playerHeight = 1.2f;
    
    public float positionSmoothTime = 0.2f;
    public float rotationSmoothSpeed = 5f;
    
    private Vector3 _velocity = Vector3.zero;
    private Quaternion? _lastTargetRotation;
    
    public void Update()
    {
        var player = RumbleModdingAPI.RMAPI.Calls.Players.GetLocalPlayer();
        if (player == null) return;
        if (parent == null) return;
        _lastTargetRotation ??= parent.transform.rotation;

        if (following)
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
                
                parent.transform.rotation = Quaternion.Slerp(
                    parent.transform.rotation,
                    desiredRotation,
                    Time.deltaTime * rotationSmoothSpeed
                );
            }
            
            newPos.y += playerHeight;
            
            float angleDifference = Quaternion.Angle((Quaternion)_lastTargetRotation, parent.transform.rotation);
            
            float dynamicSmoothTime = positionSmoothTime;
            
            if (angleDifference > 30f)
            {
                dynamicSmoothTime = 0.05f;
            }
            
            parent.transform.position = Vector3.SmoothDamp(
                parent.transform.position,
                newPos,
                ref _velocity,
                dynamicSmoothTime
            );

            _lastTargetRotation = parent.transform.rotation;
        }
    }
}

[HarmonyPatch(typeof(PlayerHandPresence), nameof(PlayerHandPresence.UpdateHandPresenceAnimationStates))]
public class Patch_PlayerHandPresence_UpdateHandPresenceAnimationStates
{
    public static PlayerHandPresence.HandPresenceInput? LHandInput;
    public static PlayerHandPresence.HandPresenceInput? RHandInput;
    
    static void Prefix(PlayerHandPresence __instance, InputManager.Hand hand, ref PlayerHandPresence.HandPresenceInput input)
    {
        if (__instance.parentController == null) return;
        
        if (__instance.parentController.ControllerType != ControllerType.Local) return;
        
        if (hand == InputManager.Hand.Left && LHandInput is { } l)
            input = l;

        if (hand == InputManager.Hand.Right && RHandInput is { } r)
            input = r;
    }
}
