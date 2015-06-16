using UnityEngine;
using System.Collections;

namespace WellFired
{
	[USequencerFriendlyName("Play Advanced Animation (Legacy)")]
	[USequencerEvent("Animation (Legacy)/Play Animation Advanced")]
	public class USPlayAdvancedAnimEvent : USEventBase {
		public AnimationClip animationClip = null;
	    public WrapMode wrapMode = WrapMode.Default;
		public AnimationBlendMode blendMode = AnimationBlendMode.Blend;
		public float playbackSpeed = 1.0f;
		public float animationWeight = 1.0f;
		public int animationLayer = 1;
		
		public bool crossFadeAnimation = false;
		
		public void Update() 
		{
	        if (wrapMode != WrapMode.Loop && animationClip)
				Duration = animationClip.length / playbackSpeed;
		}
		
		public override void FireEvent()
		{
			if(!animationClip)
			{
				Debug.Log("Attempting to play an animation on a GameObject but you haven't given the event an animation clip from USPlayAnimEvent::FireEvent");
				return;
			}
			
			Animation animation = AffectedObject.GetComponent<Animation>();
			if(!animation)
			{
				Debug.Log("Attempting to play an animation on a GameObject without an Animation Component from USPlayAnimEvent.FireEvent");
				return;
			}
			
	        animation.wrapMode = wrapMode;
			
			if(crossFadeAnimation)
		        animation.CrossFade(animationClip.name);
			else
		        animation.Play(animationClip.name);
			
			AnimationState state = animation[animationClip.name];
			if(!state)
				return;
			
			state.enabled = true;
			state.weight = animationWeight;
			state.blendMode = blendMode;
			state.layer = animationLayer;
			state.speed = playbackSpeed;
		}
		
		public override void ProcessEvent(float deltaTime)
		{
			Animation animation = AffectedObject.GetComponent<Animation>();
	
			if (!animation)
			{
				Debug.LogError("Trying to play an animation : " + animationClip.name + " but : " + AffectedObject + " doesn't have an animation component, we will add one, this time, though you should add it manually");
				animation = AffectedObject.AddComponent<Animation>();
			}
	
			if (animation[animationClip.name] == null)
			{
				Debug.LogError("Trying to play an animation : " + animationClip.name + " but it isn't in the animation list. I will add it, this time, though you should add it manually.");
				animation.AddClip(animationClip, animationClip.name);
			}
	
			AnimationState state = animation[animationClip.name];
	
	        if (!animation.IsPlaying(animationClip.name))
	        {
	            animation.wrapMode = wrapMode;
				
				if(crossFadeAnimation)
		        	animation.CrossFade(animationClip.name);
				else
		        	animation.Play(animationClip.name);
	        }
			
			state.weight = animationWeight;
			state.blendMode = blendMode;
			state.layer = animationLayer;
			state.speed = playbackSpeed;
			state.time = deltaTime * playbackSpeed;
			state.enabled = true;
			animation.Sample();
		}
		
		public override void StopEvent()
		{
			if(!AffectedObject)
				return;
			
			Animation animation = AffectedObject.GetComponent<Animation>();
	        if (animation)
				animation.Stop();
		}
		
		public override void EndEvent()
		{
			StopEvent();
		}
	}
}