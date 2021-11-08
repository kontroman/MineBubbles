using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameConfig
{
    public static float DEFAULT_BALL_SPEED = Screen.height * 3.5f;
    public static int START_ROW_COUNT = 17;
    public static int START_FILLER_ROW = 0;
    public static int DEFAULT_QUEUE_BALL_COUNT = 6;
    public static int MAX_BALL_IN_ROW = 12;
    public static int MAX_RAY_REFLECTIONS = 10;
    public static int COLORS_TO_DESTROY = 3;
    public static int WALLS_COLLIDER_SIZE = 5;

    public static float BALL_RADIUS = 50f;

    public static class Multipliers
    {
        public static float BALL_COLLIDER_MULTIPLIER = 1f; //0.8, 0.9
        public static float QUEUE_BALL_SIZE_MULTIPLIER = 1f;
    }

    public static class Angles
    {
        public static float MIN_SHOOTING_ANGLE = 15f;
    }

    public static class Spacings
    {
        public static float X_GRID_SPACING = 20f; //3
        public static float Y_GRID_SPACING = 10f; //5
    }
    public static class Limits
    {
        public static int DROP_ANIM_VX_LEFT = -5000;
        public static int DROP_ANIM_VX_RIGHT = 5001;
        public static int DROP_ANIM_VY_UPPER = 8000;
        public static int DROP_ANIM_VY_LOWER = 15001;
    }

    public static class Durations
    {
        public static float BALL_EXPLOSION_EXPAND_TIME = 0.1f;
        public static float BALL_EXPLOSION_SHRINK_TIME = 0.05f;
    }

    public static class Layers
    {
        public static int DEFAULT = 0;
        public static int IGNORE_RAYCAST = 2;
        public static int IGNORE_COLLISION = 8;
    }

    public static class Tags
    {
        public const string CEILING = "Ceiling";
        public const string WALL = "Wall";
        public const string BALL = "Ball";
    }

    public static List<string> TexturePack = new List<string>
    {
        "DefaultPack",
        "SecondPack"
    };

    public enum Types
    {
        COLORED
    }

    public enum Colors
    {
        BLUE,
        GREEN,
        LIGHT_BLUE,
        MAGENTA,
        RED,
        YELLOW
    }

    public static List<string> ColorNames = new List<string>
    {
        "blue",
        "green",
        "light_blue",
        "magenta",
        "red",
        "yellow"
    };
}
