using System.Collections.Generic;
using UnityEngine;
using cowsins.BulletHell;
using System;
using System.Collections.Concurrent; 

namespace cowsins.BulletHell
{
public class ImagePatternSpawner : MonoBehaviour
{
    [HideInInspector]public ConcurrentBag<Vector2> position = new ConcurrentBag<Vector2>();

    [HideInInspector]public int amountShot;

    private PatternSpawner spawner; 

    private Dictionary<int, ConcurrentBag<Vector2>> patternPositions = new Dictionary<int, ConcurrentBag<Vector2>>();

    private int w, h;

    private Color lastColorChecked;

    private void Awake() => spawner = GetComponent<PatternSpawner>();

    private void Start()
    {
        for (int i = 0; i < spawner.patterns.Length; i++)
        {
            if(spawner.patterns[i].pattern != null)
            {
                ConcurrentBag<Vector2> patternArray = GrabPositionsFromPattern(i);
                patternPositions.Add(Array.IndexOf(spawner.patterns, spawner.patterns[i]), patternArray);
            }
        }
        
    }

    private ConcurrentBag<Vector2> GrabPositionsFromPattern(int index)
    {
        ConcurrentBag<Vector2> position = new ConcurrentBag<Vector2>();
        position.Clear();
        w = spawner.patterns[index].pattern.width;
        h = spawner.patterns[index].pattern.height;
        for (int j = 0; j < h ; j+= spawner.patterns[index].yStep)
        {
            for (int i = 0; i < w; i+= spawner.patterns[index].xStep)
            {
                if (MatchRequirements(spawner.patterns[index].pattern.GetPixel(i, j)))
                {
                    Vector2 pos = new Vector2(i - w / 2, j - h / 2) / w * spawner.patterns[index].patternSize;
                    position.Add(pos); 
                }
            }
        }
        return position;  
    }

    public void Shoot()
    {
        int s = 1;
        foreach (var a in patternPositions[spawner.currentPattern])
        {
            // This prevents a small issue that spawns a bullet at the corner of the texture
            if (s == patternPositions[spawner.currentPattern].Count) return; 

            var projectile = PoolManager.Instance.RequestBullet();

            Vector3 vector = a;
            projectile.transform.position = vector + transform.position;
            Vector3 dir3d = transform.position - projectile.transform.position;

            float ang = Mathf.Atan2(dir3d.y, dir3d.x) * Mathf.Rad2Deg;
            projectile.transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);

            GetComponent<PatternSpawner>().SetBulletProperties(projectile, spawner.currentPattern, amountShot, 0);

            projectile.InitialSettings();
            s++;
        }
    }

    private bool MatchRequirements(Color color)
    {
        if (IsWhite(lastColorChecked))
        {
            lastColorChecked = color;
            return false;
        }
        else
        {
            lastColorChecked = color;
            return true; 
        }     
  
    }

    private bool IsWhite(Color color)
    {
        Color limitColor = new Color(.7f, .7f, .7f);
        if (color.r <= limitColor.r && color.g <= limitColor.g && color.b <= limitColor.b) return false;
        else return true; 
    }
}
}