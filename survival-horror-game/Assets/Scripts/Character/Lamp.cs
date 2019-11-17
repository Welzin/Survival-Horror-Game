using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    private void Awake()
    {
        _actualBattery = maxBattery;
        Active = false;
    }

    public void Reload()
    {
        _actualBattery = Mathf.Min(_actualBattery + batteryGainOnReload, maxBattery); 
    }

    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            if (GetComponent<MeshRenderer>())
            {
                MeshRenderer mesh = GetComponent<MeshRenderer>();
                _active = value;
                if (_actualBattery <= 0) { _active = false;  }
                mesh.enabled = _active;

            }
        }
    }

    public bool CanBeSeenByMonster(Monster monster)
    {
        return Active && IsPointInsideLampCone(monster.transform.position);
    }

    private bool IsPointInsideLampCone(Vector2 point)
    {
        /*
            O = origin of the cone
            P = Point to verify
            l = length of the cone
            w = width of the cone

            Ox = Cone's direction vector normalized
            Oy = Cone's direction normal vector normalized

            y
            |   /|
            |  / |
            | /  | w
            |/ l |
          O +----+---------------x
            |\   |
            | \  |
            |  \ |   + P
            |   \|

            First, we want the P x coord of the plan (this value is absolute) this is the distance between the point and the tip :
            Ox(P) = OP.Ox (Dot product)

            Then we need to check if P is inside the length cone :
            A = Ox(P) >= 0 AND Ox(P) <= l
            
            Now we need to know the circle width of the cone in Ox(P) (Because a cone is a infinity of circles and we know the one which interest us) :
            w(Ox(P)) = Ox(P) / l * w;

            Finally, we need the P y coord of the plan. If Oy(P) < w(Ox(P)) it means that the P is inside the cone :
            Oy(P) = ||OP - Ox(P) * Ox|| <- We take off the Ox value of OP.

            B = Oy(P) < w(Ix(P))

            Conclusion :
            If A is true and B is true, P is inside the cone

            Notes :
            - A means : Is Ox(P) inside the length cone
            - B means : Is Oy(P) inside the circle of the cone in Ox(P)
        */

        Debug.DrawRay(transform.position, transform.up);

        // O
        Vector2 tip = transform.position;
        // Ox
        Vector2 direction = transform.up;
        // l
        float coneLength = transform.localScale.y * 2;
        // w
        float coneWidth = transform.localScale.x;

        // Ox(P) = OP.Ox
        float cone_dist = Vector2.Dot((point - tip), direction);
        // w(Ox(P)) = Ox(P) / l * w
        float cone_radius = (cone_dist / coneLength) * coneWidth;
        // Oy(P) = || OP - Ox(P) * Ox ||
        float orth_distance = ((point - tip) - cone_dist * direction).magnitude;

        bool A = cone_dist <= coneLength && cone_dist >= 0;
        bool B = orth_distance < cone_radius;
        
        return A && B;
    }

    public float actualBattery {  get { return _actualBattery; } set { _actualBattery = value; } }

    public float maxBattery = 100f;
    public float consommationBySec = 10f;
    public float batteryGainOnReload;
    public float radius;

    private bool _active;
    private float _actualBattery;
}
