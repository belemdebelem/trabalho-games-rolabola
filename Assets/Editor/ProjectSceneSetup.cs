using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

[InitializeOnLoad]
public static class ProjectSceneSetup
{
    private const string ScenePath = "Assets/Scenes/JogoColeta.unity";
    private const string MaterialsPath = "Assets/Materials";

    static ProjectSceneSetup()
    {
        EditorApplication.delayCall += EnsureSceneExists;
    }

    [MenuItem("Jogo de Coleta/Criar ou recriar cena")]
    public static void CreateScene()
    {
        EnsureTagExists("lixo");
        PlayerSettings.activeInputHandler = 0;

        EnsureFolder("Assets/Scenes");
        EnsureFolder(MaterialsPath);
        CreateMaterial("AzulPiso", new Color(0.06f, 0.08f, 0.72f));
        CreateMaterial("AmareloColetavel", new Color(1f, 0.68f, 0.04f));
        CreateMaterial("BordaArena", new Color(0.72f, 0.72f, 0.68f));
        CreateMaterial("EsferaJogador", Color.white);

        var scene = UnityEditor.SceneManagement.EditorSceneManager.NewScene(
            UnityEditor.SceneManagement.NewSceneSetup.EmptyScene,
            UnityEditor.SceneManagement.NewSceneMode.Single);

        CreateArena();
        GameObject player = CreatePlayer();
        CreateCamera(player.transform);
        CreateCollectibles();
        CreateInterface();

        UnityEditor.SceneManagement.EditorSceneManager.SaveScene(scene, ScenePath);
        AddSceneToBuildSettings(ScenePath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log("Cena criada em " + ScenePath);
    }

    private static void EnsureSceneExists()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode ||
            AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath) != null)
        {
            return;
        }

        CreateScene();
    }

    private static void CreateArena()
    {
        CreateCube("Piso", new Vector3(0f, -0.25f, 0f),
            new Vector3(16f, 0.5f, 16f), "AzulPiso");
        CreateCube("Parede Norte", new Vector3(0f, 0.45f, 8f),
            new Vector3(16f, 1.4f, 0.5f), "BordaArena");
        CreateCube("Parede Sul", new Vector3(0f, 0.45f, -8f),
            new Vector3(16f, 1.4f, 0.5f), "BordaArena");
        CreateCube("Parede Leste", new Vector3(8f, 0.45f, 0f),
            new Vector3(0.5f, 1.4f, 16f), "BordaArena");
        CreateCube("Parede Oeste", new Vector3(-8f, 0.45f, 0f),
            new Vector3(0.5f, 1.4f, 16f), "BordaArena");

        var lightObject = new GameObject("Luz Direcional");
        Light light = lightObject.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.2f;
        lightObject.transform.rotation = Quaternion.Euler(50f, -30f, 0f);
    }

    private static GameObject CreatePlayer()
    {
        GameObject player = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        player.name = "Jogador";
        player.tag = "Player";
        player.transform.position = new Vector3(0f, 0.55f, 0f);
        ApplyMaterial(player, "EsferaJogador");

        Rigidbody body = player.AddComponent<Rigidbody>();
        body.mass = 1f;
        body.drag = 1f;
        player.AddComponent<PlayerController>();
        return player;
    }

    private static void CreateCamera(Transform target)
    {
        var cameraObject = new GameObject("Main Camera");
        cameraObject.tag = "MainCamera";
        Camera camera = cameraObject.AddComponent<Camera>();
        camera.fieldOfView = 55f;
        cameraObject.AddComponent<AudioListener>();

        CameraFollow follow = cameraObject.AddComponent<CameraFollow>();
        SerializedObject serializedFollow = new SerializedObject(follow);
        serializedFollow.FindProperty("target").objectReferenceValue = target;
        serializedFollow.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void CreateCollectibles()
    {
        const int count = 12;
        const float radius = 5.2f;

        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2f / count;
            Vector3 position = new Vector3(
                Mathf.Cos(angle) * radius,
                0.45f,
                Mathf.Sin(angle) * radius);
            GameObject item = CreateCube(
                "Coletavel " + (i + 1).ToString("00"),
                position,
                Vector3.one * 0.7f,
                "AmareloColetavel");
            item.tag = "lixo";
            item.GetComponent<BoxCollider>().isTrigger = true;
            item.AddComponent<Collectible>();
        }
    }

    private static void CreateInterface()
    {
        var canvasObject = new GameObject(
            "Canvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        Text scoreText = CreateText(
            canvas.transform,
            "Pontuacao",
            "Pontuacao: 0 / 12",
            new Vector2(30f, -25f),
            new Vector2(500f, 70f),
            TextAnchor.UpperLeft,
            34,
            new Color(0.2f, 1f, 0.2f));
        Text endText = CreateText(
            canvas.transform,
            "Mensagem Final",
            string.Empty,
            Vector2.zero,
            new Vector2(1200f, 120f),
            TextAnchor.MiddleCenter,
            48,
            Color.white);

        GameObject managerObject = new GameObject("GameManager");
        GameManager manager = managerObject.AddComponent<GameManager>();
        SerializedObject serializedManager = new SerializedObject(manager);
        serializedManager.FindProperty("scoreText").objectReferenceValue = scoreText;
        serializedManager.FindProperty("endMessageText").objectReferenceValue = endText;
        serializedManager.FindProperty("targetScore").intValue = 12;
        serializedManager.ApplyModifiedPropertiesWithoutUndo();
    }

    private static Text CreateText(
        Transform parent,
        string objectName,
        string initialText,
        Vector2 anchoredPosition,
        Vector2 size,
        TextAnchor alignment,
        int fontSize,
        Color color)
    {
        var textObject = new GameObject(
            objectName,
            typeof(RectTransform),
            typeof(CanvasRenderer),
            typeof(Text));
        textObject.transform.SetParent(parent, false);

        RectTransform rect = textObject.GetComponent<RectTransform>();
        rect.anchorMin = alignment == TextAnchor.UpperLeft
            ? new Vector2(0f, 1f)
            : new Vector2(0.5f, 0.5f);
        rect.anchorMax = rect.anchorMin;
        rect.pivot = alignment == TextAnchor.UpperLeft
            ? new Vector2(0f, 1f)
            : new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        Text text = textObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = initialText;
        text.fontSize = fontSize;
        text.alignment = alignment;
        text.color = color;
        text.raycastTarget = false;
        return text;
    }

    private static GameObject CreateCube(
        string objectName,
        Vector3 position,
        Vector3 scale,
        string materialName)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.name = objectName;
        cube.transform.position = position;
        cube.transform.localScale = scale;
        ApplyMaterial(cube, materialName);
        return cube;
    }

    private static void ApplyMaterial(GameObject target, string materialName)
    {
        Material material = AssetDatabase.LoadAssetAtPath<Material>(
            MaterialsPath + "/" + materialName + ".mat");
        target.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static void CreateMaterial(string materialName, Color color)
    {
        string path = MaterialsPath + "/" + materialName + ".mat";
        if (AssetDatabase.LoadAssetAtPath<Material>(path) != null)
        {
            return;
        }

        Shader shader = Shader.Find("Standard");
        if (shader == null)
        {
            shader = Shader.Find("Universal Render Pipeline/Lit");
        }

        var material = new Material(shader) { color = color };
        AssetDatabase.CreateAsset(material, path);
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
        {
            return;
        }

        string parent = System.IO.Path.GetDirectoryName(path).Replace('\\', '/');
        string folderName = System.IO.Path.GetFileName(path);
        AssetDatabase.CreateFolder(parent, folderName);
    }

    private static void EnsureTagExists(string tag)
    {
        foreach (string existingTag in InternalEditorUtility.tags)
        {
            if (existingTag == tag)
            {
                return;
            }
        }

        Object tagManagerAsset = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
        if (tagManagerAsset == null)
        {
            InternalEditorUtility.AddTag(tag);
            return;
        }

        var tagManager = new SerializedObject(tagManagerAsset);
        SerializedProperty tags = tagManager.FindProperty("tags");
        int index = tags.arraySize;
        tags.InsertArrayElementAtIndex(index);
        tags.GetArrayElementAtIndex(index).stringValue = tag;
        tagManager.ApplyModifiedProperties();
    }

    private static void AddSceneToBuildSettings(string scenePath)
    {
        var scenes = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        if (scenes.Exists(scene => scene.path == scenePath))
        {
            return;
        }

        scenes.Add(new EditorBuildSettingsScene(scenePath, true));
        EditorBuildSettings.scenes = scenes.ToArray();
    }
}
