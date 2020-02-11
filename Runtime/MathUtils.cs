using System;
using UnityEngine;

namespace CommonUtils {
	public class MathUtils {

		public static float NormalizeAngle(float angle)
		{
			// reduce the angle  
			angle = angle % 360; 

			// force it to be the positive remainder, so that 0 <= angle < 360  
			angle = (angle + 360) % 360;  

			// force into the minimum absolute value residue class, so that -180 < angle <= 180  
			if (angle > 180)  
				angle -= 360;

			return angle;
		}

		public static void TwoDimenionalLookAt(Transform transf, Vector3 targetPos) {
			var toTarget = targetPos - transf.position;
			transf.rotation = Quaternion.LookRotation(Vector3.forward, -toTarget);
			transf.Rotate(Vector3.forward, -90f);
		}

		public static Quaternion ClampQuaternion(Quaternion q, Vector3 min, Vector3 max)
		{
			return Quaternion.Euler (Mathf.Clamp(NormalizeAngle(q.eulerAngles.x), min.x, max.x), 
									 Mathf.Clamp(NormalizeAngle(q.eulerAngles.y), min.y, max.y), 
									 Mathf.Clamp(NormalizeAngle(q.eulerAngles.z), min.z, max.z));
		}

		public static Vector3 ScreenPointToLocalPointInRectangle(Canvas canvas, Vector2 position) {
			Vector2 pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,
																	position,
																	canvas.worldCamera,
																	out pos);
			return canvas.transform.TransformPoint(pos);
		}

		private static readonly System.DateTime UnixEpoch = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
		public static long UnixTimeMS()
		{
			return (long)(System.DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
		}

		public static Vector3 GetDirectionFromSpread(Quaternion rotation, float spreadAngle)
		{
			float angleOff   = spreadAngle * Mathf.Deg2Rad; 
			var   multiplier = new Vector3(UnityEngine.Random.Range(-Mathf.Sin(angleOff), Mathf.Sin(angleOff)), UnityEngine.Random.Range(-Mathf.Sin(angleOff), Mathf.Sin(angleOff)), 1.0f);
			return rotation * multiplier;
		}

		public static Vector2 RotatePointAroundOrigin(Vector2 point, Vector2 origin, float theta)
		{
			float s = Mathf.Sin(theta);
			float c = Mathf.Cos(theta);

			// translate point back to origin:
			point.x -= origin.x;
			point.y -= origin.y;

			// rotate point
			float xnew = point.x * c - point.y * s;
			float ynew = point.x * s + point.y * c;

			// translate point back:
			point.x = xnew + origin.x;
			point.y = ynew + origin.y;
			return point;
		}

		public static double DegreeToRadian(double angle) {
			return Math.PI * angle / 180.0;
		}

		public static double RadianToDegree(double angle) {
			return angle * (180.0 / Math.PI);
		}

		public static bool GetUpdatedAmmoFromReload(ref int currentAmmoInMagazine, ref int currentAmmoNotInMagazine, int ammoPerMagazine)
		{
			int ammoToTake = (int)Mathf.Min(Mathf.Clamp(currentAmmoNotInMagazine, 0, ammoPerMagazine - currentAmmoInMagazine), currentAmmoNotInMagazine);
			if (currentAmmoInMagazine == 0 && currentAmmoNotInMagazine <= ammoPerMagazine) {
				ammoToTake = currentAmmoNotInMagazine;
			}

			if (ammoToTake <= 0) {
				return false;
			}

			currentAmmoInMagazine    += ammoToTake;
			currentAmmoNotInMagazine -= ammoToTake;
			return true;
		}

		public static int WrapValue(int value, int min, int max)
		{
			return (((value - min) % (max - min)) + (max - min)) % (max - min) + min;
		}

		public static Color GetColorFromRGB255(int r, int g, int b)
		{
			return new Color ((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
		}

		public static int RandomNegPos()
		{
			return UnityEngine.Random.Range (0, 2) * 2 - 1;
		}

		public static void SetGlobalScale (Transform transform, Vector3 globalScale)
		{
			transform.localScale = Vector3.one;
			transform.localScale = new Vector3 (globalScale.x /transform.lossyScale.x, globalScale.y /transform.lossyScale.y, globalScale.z /transform.lossyScale.z);
		}

		public static byte ConvertBoolArrayToByte(bool[] source)
		{
			byte result = 0;
			// This assumes the array never contains more than 8 elements!
			int index  = 8 - source.Length;
			int length = source.Length;
			// Loop through the array
			for (int i = 0; i < length; i++)
			{
				// if the element is 'true' set the bit at that position
				if (source[i])
				{
					result |= (byte)(1 << (7 - index));
				}
				index++;
			}

			return result;
		}

		public static bool[] ConvertByteToBoolArray(byte b)
		{
			// prepare the return result
			bool[] result = new bool[8];

			// check each bit in the byte. if 1 set to true, if 0 set to false
			for (int i = 0; i < 8; i++)
				result[i] = (b & (1 << i)) == 0 ? false : true;

			// reverse the array
			//System.Array.Reverse(result);

			for (int i = 0; i < result.Length / 2; i++)
			{
				bool tmp = result[i];
				result[i] = result[result.Length - i - 1];
				result[result.Length - i             - 1] = tmp;
			}

			return result;
		}

		public static bool LineIntersectsRect(Vector2 p1, Vector2 p2, Rect r)
		{
			return LineIntersectsLine(p1, p2, new Vector2(r.x, r.y), new Vector2(r.x + r.width, r.y))                                                                ||
				   LineIntersectsLine(p1, p2, new Vector2(r.x                        + r.width, r.y), new Vector2(r.x + r.width,                    r.y + r.height)) ||
				   LineIntersectsLine(p1, p2, new Vector2(r.x                        + r.width, r.y                   + r.height), new Vector2(r.x, r.y + r.height)) ||
				   LineIntersectsLine(p1, p2, new Vector2(r.x,                                  r.y                   + r.height), new Vector2(r.x, r.y))            ||
				   (r.Contains(p1) && r.Contains(p2));
		}

		public static string Vector3ToStringVerbose(Vector3 v)
		{
			return "(" + v.x + ", " + v.y + ", " + v.z + ")";
		}

		private static bool LineIntersectsLine(Vector2 l1p1, Vector2 l1p2, Vector2 l2p1, Vector2 l2p2)
		{
			float q = (l1p1.y - l2p1.y) * (l2p2.x - l2p1.x) - (l1p1.x - l2p1.x) * (l2p2.y - l2p1.y);
			float d = (l1p2.x - l1p1.x) * (l2p2.y - l2p1.y) - (l1p2.y - l1p1.y) * (l2p2.x - l2p1.x);

			if( d == 0 )
			{
				return false;
			}

			float r = q / d;

			q = (l1p1.y - l2p1.y) * (l1p2.x - l1p1.x) - (l1p1.x - l2p1.x) * (l1p2.y - l1p1.y);
			float s = q / d;

			if( r < 0 || r > 1 || s < 0 || s > 1 )
			{
				return false;
			}

			return true;
		}

		public static Vector3 LineIntersection3D(Vector3 p1, Vector3 v1, Vector3 p2, Vector3 v2) {
			//assumes the lines actually intersect

			Vector3 cross    = Vector3.Cross(v1, v2);
			float   magCross = cross.magnitude;

			if (magCross != 0) {

				Vector3 pointDiff        = p2 - p1;
				Vector3 pointDiffCrossV2 = Vector3.Cross(pointDiff, v2);

				float a = pointDiffCrossV2.magnitude / magCross;

				return p1 + v1 * a;
			} else {
				Debug.LogError("error! lines are coplanar!");
				return Vector3.zero;
			}
		}

		public static Rect RectTransformToScreenSpace(RectTransform transform)
		{
			Vector2 size = Vector2.Scale(transform.rect.size, transform.lossyScale);
			Rect    rect = new Rect(transform.position.x, Screen.height - transform.position.y, size.x, size.y);
			rect.x -= (transform.pivot.x          * size.x);
			rect.y -= ((1.0f - transform.pivot.y) * size.y);
			return rect;
		}

		public static DateTime ConvertFromUnixTimestamp(double timestamp)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			return origin.AddSeconds(timestamp);
		}

		public static double ConvertToUnixTimestamp(DateTime date)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			TimeSpan diff   = date.ToUniversalTime() - origin;
			return Math.Floor(diff.TotalSeconds);
		}

	}
}
