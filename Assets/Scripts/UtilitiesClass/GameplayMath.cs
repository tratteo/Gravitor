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
    private const double G = 6.67E-11f;
    private const float RadToDegree = 57.3f;
    private const double Td_constant = 9E5f * (G/(c*c));

    public static float DEFAULT_RATIO = 2.4f;
    public static float RESILIENCE_RATIO = 3.12f;

    public float GetGravityTd(GameObject player, GameObstacle obstacle)
    {
        float intern = (float)(1f - ((Td_constant * obstacle.mass) / Vector3.Magnitude(player.transform.position - obstacle.transform.position)));
        float result;
        if (intern <= 0.10f)
        {
            return 4f;
        }
        else
        {
            result = (1f / Mathf.Sqrt(intern));
        }
        return result;
    }

    public float GetSpeedTd(float speed)
    {
        return 1f / (Mathf.Sqrt(1f - (speed * speed)));
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
        return 5.175E-8f * mass;
    }

    public float GetPlayerThrustForceFromPoints(int points)
    {
        return (2f * ((points - 1) * Mathf.Log(8 * points + 100))) + 250f;
    }

    public float GetAntigravityDuration(int points)
    {
        return (0.45f * points) + 2.55f;
    }

    public float GetAntigravityCooldown(int points)
    {
        return (-1.1f * points) + 31.1f;
    }

    public float GetQuantumTunnelCooldown(int points)
    {
        return (-1.25f * points) + 30.25f;
    }

    public float GetSolarflareCooldown(int points)
    {
        return (-1.1f * points) + 29.6f;
    }

    public float GetSolarflareRadius(int points)
    {
        return (-1.3f * points * points) + 41.2f * points + 127f;
    }

    public int GetGRBCost(int points)
    {
        switch(points)
        {
            case 1:
                return 1000000;
            case 2:
                return 2000000;
            default:
                return -1;
        }
    }

    public int GetGRBCooldown(int points)
    {
        switch (points)
        {
            case 1:
                return 45;
            case 2:
                return 36;
            case 3:
                return 28;
            case 4:
                return 17;
            default:
                return -1;
        }
    }

    public int GetGRBUnscaledRadius(int points)
    {
        switch (points)
        {
            case 1:
                return 45;
            case 2:
                return 65;
            case 3:
                return 80;
            case 4:
                return 110;
            default:
                return -1;
        }
    }

    public float GetDamageWithDistance(float distance)
    {
        float x = (distance / 370f) + 0.6f;
        return x * x * x;
    }

    public int GetCostFromInitCost(int points, int initCost, float ratio)
    {
        return (int)(835 * Mathf.Pow(points, ratio) + initCost - 835);
    }

    public int GetGRBSpawnExceptionTime(int points)
    {
        switch(points)
        {
            case 1:
                return 3;
            case 2:
                return 4;
            case 3:
                return 5;
            case 4:
                return 6;
            default:
                return -1;
        }
    }

    public int GetGravityPointsFromSession(float score, float properTime, Level level)
    {
        int gravityPoints = (int)(0.35f * (0.38f * score * (properTime / 300f)));
        return gravityPoints;
    }

    public int GetGravitonsFromGame(float timePlayed, float score)
    {
        float prob = 1f / ((90f / timePlayed) + 1f);
        float coeff = 1f / ((1400 / Mathf.Pow(score, 0.56f)) + 1);
        int quantity = (int)(coeff * 12f);
        float result = Random.Range(0f, 1f);
        if(prob > result)
        {
            return quantity;
        }
        return 0;
    }

    public int GetExp(int gravityPoints, GameMode.GradeObtained obt)
    { 
        int exp = gravityPoints / 4;
        switch (obt)
        {
            case GameMode.GradeObtained.BRONZE:
                exp = (int)(exp * 1.3f);
                break;
            case GameMode.GradeObtained.SILVER:
                exp = (int)(exp * 2f);
                break;
            case GameMode.GradeObtained.GOLD:
                exp = (int)(exp * 3f);
                break;
            default:
                break;
        }
        return exp;
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
