using UnityEngine;
using UnityEngine.InputSystem; // ใช้ Input แบบใหม่ของ Unity

public class Shooter : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Transform shootPoint;     // จุดยิงออก (ลาก ShootPoint มาใส่)
    [SerializeField] private GameObject target;        // เป้าหรือ Crosshair (ลาก Target มาใส่)

    [Header("Prefab")]
    [SerializeField] private Rigidbody2D bulletPrefab; // สำคัญ: ลากไฟล์กระสุนมาใส่ในช่องนี้ใหม่ใน Inspector

    void Update()
    {
        // 1. อ่านตำแหน่งเมาส์
        Vector2 screenPos = Mouse.current.position.ReadValue();

        // 2. เมื่อคลิกเมาส์ซ้าย
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            // ยิง Ray จากกล้องผ่านตำแหน่งเมาส์
            Ray ray = Camera.main.ScreenPointToRay(screenPos);

            // ตรวจสอบการชนในระนาบ 2D
            RaycastHit2D hit = Physics2D.GetRayIntersection(ray, Mathf.Infinity);

            // 3. ถ้าคลิกโดนอะไรบางอย่าง (ที่มี Collider)
            if (hit.collider != null)
            {
                // เลื่อนเป้าไปที่จุดที่คลิก
                target.transform.position = new Vector2(hit.point.x, hit.point.y);
                Debug.Log($"Hit: {hit.collider.gameObject.name}");

                // 4. คำนวณความเร็วที่ต้องใช้เพื่อให้กระสุนไปถึงเป้าหมายในเวลา 1 วินาที
                Vector2 projectileVelocity = CalculateProjectileVelocity(
                    origin: shootPoint.position,
                    target: hit.point,
                    time: 1f
                );

                // 5. สร้างกระสุน (Spawn)
                // เนื่องจากเราประกาศ bulletPrefab เป็น Rigidbody2D แล้ว
                // คำสั่ง Instantiate จะคืนค่าเป็น Rigidbody2D ให้เราโดยอัตโนมัติ
                Rigidbody2D shootBullet = Instantiate(
                    bulletPrefab,
                    shootPoint.position,
                    Quaternion.identity
                );

                // 6. ยิงกระสุนออกไปตามความเร็วที่คำนวณได้
                // ใช้ linearVelocity สำหรับ Unity 6 (หรือ velocity สำหรับเวอร์ชันเก่า)
                shootBullet.linearVelocity = projectileVelocity;
            }
        }
    }

    // ฟังก์ชันคำนวณความเร็ววิถีโค้ง (Projectile Motion)
    Vector2 CalculateProjectileVelocity(Vector2 origin, Vector2 target, float time)
    {
        Vector2 direction = target - origin;
        float h = direction.y;
        float d = direction.x;
        float g = Mathf.Abs(Physics2D.gravity.y);

        // สูตรคำนวณความเร็วต้นแยกแกน X และ Y
        float vx = d / time;
        float vy = (h / time) + (0.5f * g * time);

        return new Vector2(vx, vy);
    }
}