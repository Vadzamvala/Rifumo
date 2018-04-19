using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrendDetector
{
    //https://stackoverflow.com/questions/563198/whats-the-most-efficent-way-to-calculate-where-two-line-segments-intersect

    public struct Point
    {
        public int x;
        public int y;
    };

    public static class LineIntersect
    {
        // Given three colinear points p, q, r, the function checks if
        // point q lies on line segment 'pr'
        public static bool OnSegment(Point p, Point q, Point r)
        {
            if (q.x <= Math.Max(p.x, r.x) && q.x >= Math.Min(p.x, r.x) &&
                q.y <= Math.Max(p.y, r.y) && q.y >= Math.Min(p.y, r.y))
                return true;

            return false;
        }

        // To find orientation of ordered triplet (p, q, r).
        // The function returns following values
        // 0 --> p, q and r are colinear
        // 1 --> Clockwise
        // 2 --> Counterclockwise
        public static int Orientation(Point p, Point q, Point r)
        {
            // See https://www.geeksforgeeks.org/orientation-3-ordered-points/ for details of below formula.
            int val = (q.y - p.y) * (r.x - q.x) -
                      (q.x - p.x) * (r.y - q.y);

            if (val == 0) return 0;  // colinear

            return (val > 0) ? 1 : 2; // clock or counterclock wise
        }

        // The main function that returns true if line segment 'p1q1'
        // and 'p2q2' intersect.
        public static bool DoIntersect(Point p1, Point q1, Point p2, Point q2)
        {
            // Find the four orientations needed for general and
            // special cases
            int o1 = Orientation(p1, q1, p2);
            int o2 = Orientation(p1, q1, q2);
            int o3 = Orientation(p2, q2, p1);
            int o4 = Orientation(p2, q2, q1);

            // General case
            if (o1 != o2 && o3 != o4)
                return true;

            // Special Cases
            // p1, q1 and p2 are colinear and p2 lies on segment p1q1
            if (o1 == 0 && OnSegment(p1, p2, q1)) return true;

            // p1, q1 and p2 are colinear and q2 lies on segment p1q1
            if (o2 == 0 && OnSegment(p1, q2, q1)) return true;

            // p2, q2 and p1 are colinear and p1 lies on segment p2q2
            if (o3 == 0 && OnSegment(p2, p1, q2)) return true;

            // p2, q2 and q1 are colinear and q1 lies on segment p2q2
            if (o4 == 0 && OnSegment(p2, q1, q2)) return true;

            return false; // Doesn't fall in any of the above cases
        }

        /*
         // Returns 1 if the lines intersect, otherwise 0. In addition, if the lines 
        // intersect the intersection point may be stored in the floats i_x and i_y.
        int get_line_intersection(float p0_x, float p0_y, float p1_x, float p1_y, 
            float p2_x, float p2_y, float p3_x, float p3_y, float *i_x, float *i_y)
        {
            float s02_x, s02_y, s10_x, s10_y, s32_x, s32_y, s_numer, t_numer, denom, t;
            s10_x = p1_x - p0_x;
            s10_y = p1_y - p0_y;
            s02_x = p0_x - p2_x;
            s02_y = p0_y - p2_y;
 
            s_numer = s10_x * s02_y - s10_y * s02_x;
            if (s_numer < 0)
                return 0; // No collision
 
            s32_x = p3_x - p2_x;
            s32_y = p3_y - p2_y;
            t_numer = s32_x * s02_y - s32_y * s02_x;
            if (t_numer < 0)
                return 0; // No collision
 
            denom = s10_x * s32_y - s32_x * s10_y;
            if (s_numer > denom || t_numer > denom)
                return 0; // No collision
 
            // Collision detected
            t = t_numer / denom;
            if (i_x != NULL)
                *i_x = p0_x + (t * s10_x);
            if (i_y != NULL)
                *i_y = p0_y + (t * s10_y);
 
            return 1;
        }


        python version
        --------------
        def find_intersection( p0, p1, p2, p3 ) :

            s10_x = p1[0] - p0[0]
            s10_y = p1[1] - p0[1]
            s32_x = p3[0] - p2[0]
            s32_y = p3[1] - p2[1]

            denom = ( (s10_x * s32_y) - (s32_x * s10_y) )

            if denom == 0 : return None # collinear

            denom_is_positive = denom > 0

            s02_x = p0[0] - p2[0]
            s02_y = p0[1] - p2[1]

            s_numer = s10_x * s02_y - s10_y * s02_x

            if (s_numer < 0) == denom_is_positive : return None # no collision

            t_numer = s32_x * s02_y - s32_y * s02_x

            if (t_numer < 0) == denom_is_positive : return None # no collision

            if (s_numer > denom) == denom_is_positive or (t_numer > denom) == denom_is_positive : return None # no collision


            # collision detected

            t = t_numer / denom

            intersection_point = [ p0[0] + (t * s10_x), p0[1] + (t * s10_y) ]


            return intersection_point
         */
    }
}
