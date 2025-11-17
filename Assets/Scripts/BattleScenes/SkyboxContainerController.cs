using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SkyboxRotationController : MonoBehaviour
{
    private const string PREF_KEY = "SkyboxRotation_LastValue";

    [Header("Rotação manual (graus)")]
    [Range(0f, 360f)]
    public float manualRotation = 0f;

    [Header("Rotação automática")]
    public bool autoRotate = false;
    public float rotationSpeed = 1f;

    private float currentRotation;

    void OnEnable()
    {
        // Carrega a última rotação salva
        currentRotation = PlayerPrefs.GetFloat(PREF_KEY, manualRotation);
        ApplyRotation();
    }

    void Update()
    {
        if (RenderSettings.skybox == null)
            return;

        if (autoRotate)
        {
            currentRotation += rotationSpeed * Time.deltaTime;
            if (currentRotation > 360f)
                currentRotation -= 360f;
            ApplyRotation();
        }
        else
        {
            if (Mathf.Abs(currentRotation - manualRotation) > 0.01f)
            {
                currentRotation = manualRotation;
                ApplyRotation();
            }
        }
    }

    private void ApplyRotation()
    {
        if (RenderSettings.skybox.HasProperty("_Rotation"))
        {
            RenderSettings.skybox.SetFloat("_Rotation", currentRotation);
            DynamicGI.UpdateEnvironment();

#if UNITY_EDITOR
            SceneView.RepaintAll();
#endif
        }
        else
        {
            Debug.LogWarning("O material da Skybox não possui o parâmetro '_Rotation'. Use Skybox/Panoramic ou Skybox/6 Sided.");
        }
    }

    void OnDisable()
    {
        // Salva o último valor de rotação ao desativar
        PlayerPrefs.SetFloat(PREF_KEY, currentRotation);
        PlayerPrefs.Save();
    }

#if UNITY_EDITOR
    // Atualiza em tempo real no editor mesmo sem o Play Mode
    void OnValidate()
    {
        if (!Application.isPlaying)
        {
            currentRotation = manualRotation;
            ApplyRotation();
        }
    }
#endif
}