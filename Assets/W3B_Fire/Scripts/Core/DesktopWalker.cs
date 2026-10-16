using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

// Walk round the room without a headset: WASD to move, hold right mouse to look.
// Put it on the object that has the Audio Listener (usually the Main Camera).
public class DesktopWalker : MonoBehaviour
{
    public float speed = 2f;
    public float lookSpeed = 0.15f;

    float yaw, pitch;

    void Start()
    {
        var e = transform.eulerAngles;
        yaw = e.y;
        pitch = e.x > 180f ? e.x - 360f : e.x;
    }

    void Update()
    {
        Vector2 move = Vector2.zero, look = Vector2.zero;
        bool looking = false;

#if ENABLE_INPUT_SYSTEM
        var k = Keyboard.current;
        var m = Mouse.current;
        if (k != null)
        {
            if (k.wKey.isPressed) move.y += 1f;
            if (k.sKey.isPressed) move.y -= 1f;
            if (k.dKey.isPressed) move.x += 1f;
            if (k.aKey.isPressed) move.x -= 1f;
        }
        if (m != null) { looking = m.rightButton.isPressed; look = m.delta.ReadValue(); }
#else
        move = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        looking = Input.GetMouseButton(1);
        look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * 10f;
#endif

        if (looking)
        {
            yaw += look.x * lookSpeed;
            pitch = Mathf.Clamp(pitch - look.y * lookSpeed, -80f, 80f);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }

        Vector3 forward = transform.forward; forward.y = 0f; forward.Normalize();
        Vector3 right = transform.right; right.y = 0f; right.Normalize();
        transform.position += (forward * move.y + right * move.x) * speed * Time.deltaTime;
    }
}
