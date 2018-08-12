////////////////////////////////////////////////////////////////////////////////
// Filename: applicationclass.h
////////////////////////////////////////////////////////////////////////////////
#ifndef _APPLICATIONCLASS_H_
#define _APPLICATIONCLASS_H_

#include <chrono>

/////////////
// GLOBALS //
/////////////
const bool FULL_SCREEN = true;
const bool VSYNC_ENABLED = true;
const float SCREEN_DEPTH = 1000.0f;
const float SCREEN_NEAR = 0.1f;


///////////////////////
// MY CLASS INCLUDES //
///////////////////////
#include "inputclass.h"
#include "d3dclass.h"
#include "cameraclass.h"
#include "terrainclass.h"
#include "timerclass.h"
#include "positionclass.h"
#include "fpsclass.h"
#include "cpuclass.h"
#include "fontshaderclass.h"
#include "textclass.h"
#include "terrainshaderclass.h"
#include "lightclass.h"
#include "skydomeclass.h"
#include "skydomeshaderclass.h"
#include "modelclass.h"
#include "modelshaderclass.h"
#include "trackclass.h"
#include "trackshaderclass.h"
#include "flyinfoclass.h"

////////////////////////////////////////////////////////////////////////////////
// Class name: ApplicationClass
////////////////////////////////////////////////////////////////////////////////
class ApplicationClass
{
public:
	ApplicationClass();
	ApplicationClass(const ApplicationClass&);
	~ApplicationClass();

	bool Initialize(HINSTANCE, HWND, int, int);
	void Shutdown();
	bool Frame(HWND);

private:
	bool HandleInput(float);
	bool RenderGraphics();

private:
	InputClass* m_Input;
	D3DClass* m_Direct3D;
	CameraClass* m_Camera;
	TerrainClass* m_Terrain;
	TimerClass* m_Timer;
	PositionClass* m_Position;
	FpsClass* m_Fps;
	CpuClass* m_Cpu;
	FontShaderClass* m_FontShader;
	TextClass* m_Text;
	TerrainShaderClass* m_TerrainShader;
	LightClass* m_Light;
	SkyDomeClass* m_SkyDome;
	SkyDomeShaderClass* m_SkyDomeShader;
	ModelClass *m_Model;
	ModelShaderClass* m_ModelShader;
	TrackClass* m_Track;
	TrackShaderClass* m_TrackShader;
	XMFLOAT3 m_center;
	chrono::system_clock::time_point m_lastTimePoint;
	chrono::system_clock::time_point m_lastIDCheck;
	FlyInfoClass::FlightInfo* m_fInfo;
	XMFLOAT3 m_changeRate;
	XMFLOAT3 m_currentPosition;
	chrono::duration<long long, std::milli> m_remainedTime;
	FlyInfoClass* m_flightInfo;
};

#endif