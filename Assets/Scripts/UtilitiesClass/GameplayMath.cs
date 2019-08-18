using UnityEngine;

public class GameplayMath
{

    private static GameplayMath instance = null;
    public static GameplayMath GetInstance()
    {
        if (instance == null)
            instance = new GameplayMath();
        return instance;
    }
    private GameplayMath() { }

    private const float c = 299.792458f;
    private const double G = 2E-10f;
    private const float RadToDegree = 57.3f;
    private const double TD_CONST = 2f * G/(c*c);

    public float GetSpawnRateFromTime(int seconds)
    {
        return (-865f / ((seconds >> 4) + 45)) + 20.5f;
        //return 0.062f * seconds + 2f;
    }

    public float GetTimeDistortion(GameObject player, GameObject obstacle)
    {
        float intern = (float)(1 - (TD_CONST * obstacle.GetComponent<GameObstacle>().mass / Vector3.Magnitude(player.transform.position - obstacle.transform.position)));
        return 8E6f * (1f / Mathf.Sqrt(intern));

        //double numerator = 20E+6 * G * obstacle.GetComponent<GameObstacle>().mass;
        //double inverseDenominator = 1f / (Vector3.Magnitude(player.transform.position - obstacle.transform.position) * c * c);
        //return (float)(1 + (numerator * inverseDenominator));
    }

    public float GetGravityIntensity(GameObject player, GameObject obstacle)
    {
        float distance = Vector3.Magnitude(player.transform.position - obstacle.transform.position);
        return (float)(G * (player.GetComponent<Rigidbody>().mass * obstacle.GetComponent<GameObstacle>().mass) / (distance * distance));
    }

    public float GetObstacleMass(GameObstacle obstacle)
    {
        float radius = obstacle.targetScale / 2f;
        return (radius * radius * radius) * (4 / 3f) * Mathf.PI * obstacle.density;
    }

    public float GetBonusPointsFromObstacleMass(float mass)
    {
        return (0.0035f * mass) / 20000;
    }

    public float GetPlayerThrustForceFromPoints(int points)
    {
        return ((points - 1) * Mathf.Log(5 * points + 79)) + 180f;
    }

    public float GetAntigravityDuration(int points)
    {
        return (0.7f * points) + 2.5f;
    }

    public float GetAntigravityCooldown(int points)
    {
        return (-1.5f * points) + 29.5f;
    }

    public float GetQuantumTunnelCooldown(int points)
    {
        return (-1.2f * points) + 25f;
    }

    public float GetSolarflareCooldown(int points)
    {
        return (-1.8f * points) + 32f;
    }

    public float GetSolarflareRadius(int points)
    {
        return (-1.3f * points * points) + 41.2f * points + 127f;
    }

    public float GetDamageWithDistance(float distance)
    {
        float x = (distance / 375f) + 0.6f;
        return x * x * x;
    }

    public int GetCostFromInitCost(int points, int initCost)
    {
        return (int)(262 * Mathf.Pow(points, 2.3f) + initCost - 262);
    }

    public float arctan(float x, float y)
    {
        if (x >= 0)
        {
            if(y >= 0)
            {
                if (y == 0) return -90f;

                return -Mathf.Atan(x / y) * RadToDegree;
            }
            else 
            {
                return -180f + (Mathf.Atan(x / -y) * RadToDegree);
            }
        }
        else
        {
            if (y >= 0)
            {
                if (y == 0) return 90f;

                return Mathf.Atan(-x / y) * RadToDegree;
            }
            else 
            {
                return 180f - (Mathf.Atan(x / y) * RadToDegree);
            }
        }
    }
}
