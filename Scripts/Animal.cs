using Godot;
using System;

public partial class Animal : RigidBody3D
{
    public Area3D playerDetectArea;
    public CollisionShape3D detectAreaShape;
    public MeshInstance3D detectAreaDebugMesh;
    public bool inside = false;
    public float moveDirection = 0f;

    [Export]
    public float detectRadius = 5f;

    [Export]
    public float roamTime = 3000f;

    [Export]
    public float afkTime = 5000f;

    [Export]
    public float Speed = 5f;

    [Export]
    public float ScaredSpeed = 8f;

    [Export]
    public float velocityLerp = 0.15f;

    [Export]
    public string runAnimBlend;

    [Export]
    public string dieAnimBlend;

    [Export]
    public string runTimescale;

    [Export]
    public float animationSpeed = 2.5f;



    public RandomNumberGenerator random;
    public Node3D model;
    
    public AnimationPlayer animationPlayer;
    public AnimationTree animationTree;
    public CharacterBody3D player;
    public Node3D playerModel;

    bool roaming = true;
    float lastTimer = 0;
    bool dead = false;

    public override void _Ready()
    {
        playerDetectArea = GetNode<Area3D>("PlayerDetectArea");
        detectAreaShape = playerDetectArea.GetNode<CollisionShape3D>("CollisionShape3D");
        detectAreaShape.Shape.Set("radius", detectRadius);
        detectAreaDebugMesh = playerDetectArea.GetNode<MeshInstance3D>("MeshInstance3D");
        detectAreaDebugMesh.Mesh.Set("top_radius", detectRadius);
        playerDetectArea.AreaEntered += OnAreaEntered;
        playerDetectArea.AreaExited += OnAreaExited;
        random = new RandomNumberGenerator();
        random.Randomize();
        model = GetNode<Node3D>("Model");
        animationPlayer = model.GetNode<AnimationPlayer>("AnimationPlayer");
        animationTree = model.GetNode<AnimationTree>("AnimationTree");
        RandomDirection();
        lastTimer = Time.GetTicksMsec();
        SetProcess(false);
        SetProcessInput(false);
    }

    public void OnAreaEntered(Area3D area)
    {
        if (area.GetParent() is CharacterBody3D _player)
        {
            RandomDirection();
            player = _player;
            playerModel = player.GetNode<Node3D>("Model");
            inside = true;
        }
    }

    public void OnAreaExited(Area3D area)
    {
        if (area.GetParent() is CharacterBody3D)
        {
            inside = false;
        }
    }

    public void RandomDirection()
    {
        moveDirection = Mathf.DegToRad(random.RandfRange(0, 360));
    }

    public override void _PhysicsProcess(double delta)
	{
        if (Time.GetTicksMsec() - lastTimer >= (roaming ? roamTime : afkTime) || inside)
        {
            if (inside)
            {
                roaming = true;
            }
            else
            {
                roaming = !roaming;
                if (roaming) RandomDirection();
                lastTimer = Time.GetTicksMsec();
            }
        }
		Vector2 inputDir = new Vector2(Mathf.Sin(moveDirection), Mathf.Cos(moveDirection));
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		Vector3 newRotation = this.Rotation;
		Vector3 Velocity = this.LinearVelocity;
		if (direction != Vector3.Zero && roaming)
		{
			Velocity.X = Mathf.Lerp(Velocity.X, direction.X * (inside ? ScaredSpeed : Speed), velocityLerp);
			Velocity.Z = Mathf.Lerp(Velocity.Z, direction.Z * (inside ? ScaredSpeed : Speed), velocityLerp);
		}
		else
		{
			Velocity.X = Mathf.Lerp(Velocity.X, 0, velocityLerp);
			Velocity.Z = Mathf.Lerp(Velocity.Z, 0, velocityLerp);
		}
		newRotation.Y = Mathf.LerpAngle(newRotation.Y, moveDirection, velocityLerp / 2);
		Rotation = newRotation;
		model.Rotation = newRotation;
		this.LinearVelocity = Velocity;

        animationTree.Set(runTimescale, Velocity.Length() / animationSpeed);

        float animAmount = Velocity.Length() / (inside ? ScaredSpeed : Speed);
        animationTree.Set(runAnimBlend, animAmount);
	}

    public void Kill()
    {
        OnKill();
        LinearVelocity = new Vector3(0, 0, 0);
        animationTree.Set(runAnimBlend, 0);
        animationTree.Set(dieAnimBlend, 1);
        SetPhysicsProcess(false);
        this.CollisionMask = 2;
        this.CollisionLayer = 2;
    }

    public virtual void OnKill()
    {

    }
}