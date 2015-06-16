/*
 * Copyright 2012 Applifier
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *    http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 *
 */

#ifndef UNITY_VERSION
#include "EveryplayConfig.h"
#define UNITY_VERSION EVERYPLAY_UNITY_VERSION
#endif

#if !defined(UNITY_VERSION) || UNITY_VERSION == 0
#error "Everyplay integration error"
#error "Try rebuilding and replacing the Xcode project from scratch in Unity"
#error "Make sure all Assets/Editor/PostprocessBuildPlayer* files are run correctly"
#endif

#if UNITY_VERSION >= 410
#include "GlesHelper.h"
#include "EAGLContextHelper.h"
#else
#include "iPhone_GlesSupport.h"
#endif

#ifdef EVERYPLAY_GLES_WRAPPER

#include "iPhone_Profiler.h"
#if UNITY_VERSION >= 420
#import "UnityAppController.h"
#else
#import "AppController.h"
#endif
#if UNITY_VERSION >= 400
#import "iPhone_OrientationSupport.h"
#endif
#if UNITY_VERSION >= 430
#import "UnityInterface.h"
#endif

#if UNITY_VERSION < 450

// iPhone_View.h
extern UIViewController* UnityGetGLViewController();
extern UIView* UnityGetGLView();

// AppController.m
#if UNITY_VERSION < 410
extern EAGLSurfaceDesc _surface;
#else
extern "C" const UnityRenderingSurface* UnityDisplayManager_MainDisplayRenderingSurface();
#endif
extern id _displayLink;

extern "C" void InitEAGLLayer(void* eaglLayer, bool use32bitColor);
#if UNITY_VERSION < 410
extern "C" bool AllocateRenderBufferStorageFromEAGLLayer(void* eaglLayer);
extern "C" void DeallocateRenderBufferStorageFromEAGLLayer();
#endif

// iPhone_GlesSupport.cpp
extern bool	UnityIsCaptureScreenshotRequested();
extern void	UnityCaptureScreenshot();

// libiPhone
extern GLint gDefaultFBO;

extern bool UnityUse32bitDisplayBuffer();
extern int UnityGetDesiredMSAASampleCount(int defaultSampleCount);
extern void UnityGetRenderingResolution(unsigned* w, unsigned* h);

extern void UnityBlitToSystemFB(unsigned tex, unsigned w, unsigned h, unsigned sysw, unsigned sysh);

extern void UnityPause(bool pause);

#if UNITY_VERSION == 350

enum EnabledOrientation
{
    kAutorotateToPortrait = 1,
    kAutorotateToPortraitUpsideDown = 2,
    kAutorotateToLandscapeLeft = 4,
    kAutorotateToLandscapeRight = 8
};

enum ScreenOrientation
{
    kScreenOrientationUnknown,
    portrait,
    portraitUpsideDown,
    landscapeLeft,
    landscapeRight,
    autorotation,
    kScreenOrientationCount
};
#else
extern void UnityGLInvalidateState();
#endif

#if UNITY_VERSION >= 430
extern ScreenOrientation ConvertToUnityScreenOrientation(UIInterfaceOrientation hwOrient, EnabledOrientation* outAutorotOrient);
extern void UnitySetScreenOrientation(int/*ScreenOrientation*/ orientation);
#else
extern ScreenOrientation ConvertToUnityScreenOrientation(int hwOrient, EnabledOrientation* outAutorotOrient);
extern void UnitySetScreenOrientation(ScreenOrientation orientation);
#endif

#endif // UNITY_VERSION < 450

#endif
