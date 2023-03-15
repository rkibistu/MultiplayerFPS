using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationController : MonoBehaviour {

    // PUBLIC MEMBERS

    public int RunBlendTree => _runBlendTree;
    public int DeathFlyingBack => _deathFlyingBack;


    // PRIVATE MEMBERS
    private Animator _animator;

    // Animator STATES (hashes)
    private int _runBlendTree;
    private int _jumpUp;
    private int _jumpLoop;
    private int _jumpDown;
    private int _deathFlyingBack;

    // Animator PARAMETRS (hashes)
    private int _velocityX;
    private int _velocityZ;
    private int _velocityY;
    private int _isJumpingUp;
    private int _isJumpLooping;
    private int _isJumpingDown;

    // MONOBEHAVIOUR INTERFACE

    void Awake() {
        _animator = GetComponent<Animator>();


        //aniamtor states
        _runBlendTree = Animator.StringToHash("RunBlendTree");
        _jumpUp = Animator.StringToHash("JumpUp");
        _jumpLoop = Animator.StringToHash("JumpLoop");
        _jumpDown = Animator.StringToHash("JumpDown");
        _deathFlyingBack = Animator.StringToHash("DeathFlyingBack");

        // aniamtor aprameters;
        _velocityX = Animator.StringToHash("VelocityX");
        _velocityZ = Animator.StringToHash("VelocityZ");
        _velocityY = Animator.StringToHash("VelocityY");
        _isJumpingUp = Animator.StringToHash("IsJumpingUp");
        _isJumpLooping = Animator.StringToHash("IsJumpLooping");
        _isJumpingDown = Animator.StringToHash("IsJumpingDown");

    }

    // PUBLIC METHODS

    public void Play(int stateHash) {

        //daca starea dorita e diferita de starea curenta a animatorului -> Play it!
        if (_animator.GetCurrentAnimatorStateInfo(0).shortNameHash != stateHash) {


            _animator.Play(stateHash);
        }
    }
    public void PlayJumpUp() {

        //Play aniamtia de inceput de saritura
        //         -> ar trebui apelata mereu cand se apasa butonul de jump pentru intirerea sariturii
        _animator.SetBool(_isJumpingUp, true);

        _animator.SetBool(_isJumpingDown, false);
        _animator.SetBool(_isJumpLooping, false);
    }
    public void PlayJumpLoop() {


        //Play aniamtia de Jump Loop
        //      -> tranzitia spre aceasta stare se face automat dupa termianarea starii JumpUp
        //      -> apelarea cestei functii e necesara in cazul in care caracterul incepe sa cada,
        //              fara sa fi sarit. (merge inainte si da de o groapa)
        //E echivalentul unei stari de falling
        _animator.SetBool(_isJumpLooping, true);

        _animator.SetBool(_isJumpingUp, false);
        _animator.SetBool(_isJumpingDown, false);
    }
    public void PlayJumpDown() {

        //Play animatia de Jump Down
        //      -> ar trebui apelata oricand se doreste animatia de Landing(jump down)
        //                  de obicei dupa starea JumpLoop
        _animator.SetBool(_isJumpingDown, true);

        _animator.SetBool(_isJumpingUp, false);
        _animator.SetBool(_isJumpLooping, false);
    }

    public void SetRunBlendTree(Vector3 speed, float maxSpeed, Transform player) {

        //Aduce valorile vitezei in intervalul (-1,1)
        //Apoi seteaza parametrii aniamtorului pentru RunBlendTree

        //calculeaza directia, tinand cont de orientarea playerului
        Vector3 dir = player.forward * speed.x + player.right * speed.z;

        //normalizeaza
        float speedNormalizedX = dir.x / maxSpeed;
        float speedNormalizedZ = dir.z / maxSpeed;

        //rotunjeste valorile in apropierea punctelor importante
        speedNormalizedX = (speedNormalizedX < -1) ? -1 : speedNormalizedX;
        speedNormalizedX = (speedNormalizedX > 1) ? 1 : speedNormalizedX;
        speedNormalizedX = (speedNormalizedX > -0.05 && speedNormalizedX < 0.05) ? 0 : speedNormalizedX;

        speedNormalizedZ = (speedNormalizedZ < -1) ? -1 : speedNormalizedZ;
        speedNormalizedZ = (speedNormalizedZ > 1) ? 1 : speedNormalizedZ;
        speedNormalizedZ = (speedNormalizedZ > -0.05 && speedNormalizedZ < 0.05) ? 0 : speedNormalizedZ;

        //seteaza parametrii aniamtorului
        _animator?.SetFloat(_velocityX, speedNormalizedZ);
        _animator?.SetFloat(_velocityZ, speedNormalizedX);
    }

    

    // PRIVATE METHODS

  
}
