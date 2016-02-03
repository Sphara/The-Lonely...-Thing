using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(IPlatformGuideFunction))]
public class FunctionPlatformController : PlatformController {

	IPlatformGuideFunction func;

	public override void Start () {

		func = GetComponent<IPlatformGuideFunction> ();

		base.Start ();
	}

	protected override void Update () {

		movement = func.GetMovement ();
		base.Update ();
	}
}
