using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class waterShapeController : MonoBehaviour
{
    [SerializeField]
    private float springStiffness = 0.1f;
    [SerializeField]
    private List<waterBounce> springs = new();
    [SerializeField]
    private float dampening = 0.3f;

    public float spread = 0.006f;

    private int CornersCount = 2;
    [SerializeField]
    private SpriteShapeController spriteShapeController;
    [SerializeField]
    private int WaveCount = 6;

    private void FixedUpdate()
    {
        foreach(waterBounce waterBounceComponent in springs)
        {
            waterBounceComponent.WaveSpringUpdate(springStiffness, dampening);
        }
        UpdateSprings();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void UpdateSprings()
    {
        int count = springs.Count;
        float[] left_deltas = new float[count];
        float[] right_deltas = new float[count];
        for(int i = 0; i < count; i++)
        {
            if (i > 0)
            {
                left_deltas[i] = spread * (springs[i].height - springs[i - 1].height);
                springs[i - 1].velocity += left_deltas[i];
            }
            if (i < springs.Count -1)
            {
                right_deltas[i] = spread * (springs[i].height - springs[i + 1].height);
                springs[i + 1].velocity += right_deltas[i];
            }
        }
    }

    private void Splash (int index, float speed)
    {
        if (index >= 0 && index < springs.Count)
        {
            springs[index].velocity += speed;
        }
    }

    private void SetWaves()
    {
        Spline waterSpline = spriteShapeController.spline;
        int waterPoitsCount = waterSpline.GetPointCount();
        
        for (int i = CornersCount; i < waterPoitsCount - CornersCount; i++)
        {
            waterSpline.RemovePointAt(CornersCount);
        }

        Vector3 waterTopLeftCorner = waterSpline.GetPosition(1);
        Vector3 waterTopRightCorner = waterSpline.GetPosition(2);
        float waterWidth = waterTopRightCorner.x - waterTopLeftCorner.x;
        float spacingPerWave = waterWidth / (WaveCount + 1);

        for(int i = WaveCount; i > 0; i--)
        {
            int index = CornersCount;
            float xPosition = waterTopLeftCorner.x + (spacingPerWave * i);
            Vector3 wavePoint = new Vector3(xPosition, waterTopLeftCorner.y, waterTopLeftCorner.z);
            waterSpline.InsertPointAt(index, wavePoint);
            waterSpline.SetHeight(index, 0.01f);
            waterSpline.SetCorner(index, false);
        }
;    }

    /*private void CreateSprings(Spline waterSpline)
    {
        springs = new();
        for (int i = 0; i <= WaveCount + 1; i++)
        {
            int index = i + 1;
            GameObject wavePoint = Instantiate(wavePointPref, wavePoints.transform, false);
            wavePoint.transform.localPosition = waterSpline.GetPosition(index);
            WaterSpring waterspring = wvavePoint.GetComponent<waterspring>();
        }
    }*/
}
