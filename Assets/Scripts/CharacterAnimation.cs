using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{
    private const string Grounded = "Grounded";
    private const string Speed = "Speed";

    [SerializeField] private Animator _animator;
    [SerializeField] private CheckFly _checkFly;
    [SerializeField] private Character _character;
   
    private void Update()
    {
        Vector3 LocalVelocity = _character.transform.InverseTransformVector(_character.velocity);
        float speed = LocalVelocity.magnitude / _character.speed;
        float sign = Mathf.Sign(LocalVelocity.z);

        _animator.SetFloat(Speed, speed * sign);
        _animator.SetBool(Grounded, _checkFly.IsFly == false);
    }
}
