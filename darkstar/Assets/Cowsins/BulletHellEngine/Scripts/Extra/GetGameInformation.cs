#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using TMPro;
using cowsins.BulletHell; 
namespace cowsins.BulletHell
{
public class GetGameInformation : MonoBehaviour
{
    //FPS
    [SerializeField,Range(30,400)] private int limitFPS; 

    public bool showFps;

    public bool showMinimumAndMaximumFps;

    [SerializeField, Range(.01f, 1f)] private float fpsRefreshRate;

    [SerializeField] private GameObject fpsObject, minFpsObject, maxFpsObject;

    [SerializeField] private Color appropriateValueColor, intermediateValueColor, badValueColor;

    private float fpsTimer;

    private float fps, minFps, maxFps;

    private Transform container;

    private void Start()
    {
        container = transform.GetChild(0);
        if (showFps)
            fpsTimer = fpsRefreshRate;
        else
            Destroy(fpsObject);

        if (!showMinimumAndMaximumFps)
        {
            Destroy(minFpsObject);
            Destroy(maxFpsObject);
        }
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = limitFPS;
    }

    private void Update()
    {
        if (showFps)
        {
            fpsTimer -= Time.deltaTime;
            if (fpsTimer <= 0)
            {
                fps = 1.0f / Time.deltaTime;

                if (fps < minFps) minFps = fps;
                if (fps > maxFps) maxFps = fps;
                fpsTimer = fpsRefreshRate;
            }
            fpsObject.GetComponent<TextMeshProUGUI>().text = "FPS: " + fps.ToString("F0");

            UpdateColorValues(fpsObject.GetComponent<TextMeshProUGUI>(), fps);

            if (showMinimumAndMaximumFps)
            {
                if(minFpsObject != null)
                {
                    minFpsObject.GetComponent<TextMeshProUGUI>().text = "MIN FPS: " + minFps.ToString("F0"); 
                    UpdateColorValues(minFpsObject.GetComponent<TextMeshProUGUI>(), minFps);
                }
                if (maxFpsObject != null)
                {
                    maxFpsObject.GetComponent<TextMeshProUGUI>().text = "MAX FPS: " + maxFps.ToString("F0"); 
                    UpdateColorValues(maxFpsObject.GetComponent<TextMeshProUGUI>(), maxFps);
                }

            }
        }
    }

    private void UpdateColorValues(TextMeshProUGUI txt, float fpss)
    {
        if (fpss < 15) txt.color = badValueColor;
        else if (fpss > 45) txt.color = appropriateValueColor;
        else txt.color = intermediateValueColor;
    }

}

#if UNITY_EDITOR
[System.Serializable]
[CustomEditor(typeof(GetGameInformation))]
public class GetGameInformatioEditor : Editor
{

    override public void OnInspectorGUI()
    {
        serializedObject.Update();
        GetGameInformation myScript = target as GetGameInformation;

        EditorGUILayout.LabelField("FPS", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("limitFPS"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("showFps"));
        if (myScript.showFps)
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fpsRefreshRate"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("fpsObject"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("showMinimumAndMaximumFps"));
            if (myScript.showMinimumAndMaximumFps)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty("minFpsObject"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("maxFpsObject"));
            }
        }
        EditorGUILayout.Space(10f);
        EditorGUILayout.LabelField("COLOR", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("appropriateValueColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("intermediateValueColor"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("badValueColor"));

        serializedObject.ApplyModifiedProperties();

    }
}
#endif
}