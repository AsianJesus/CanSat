////////////////////////////////////////////////////////////////////////////////
// Filename: modelclass.h
////////////////////////////////////////////////////////////////////////////////
#ifndef _MODELCLASS_H_
#define _MODELCLASS_H_


//////////////
// INCLUDES //
//////////////
#include <d3d11.h>
#include <directxmath.h>
#include <vector>
using namespace DirectX;
using namespace std;

////////////////////////////////////////////////////////////////////////////////
// Class name: ModelClass
////////////////////////////////////////////////////////////////////////////////
class ModelClass
{
private:
	struct VertexType
	{
		XMFLOAT3 position;
#ifdef FINAL
		XMFLOAT3 normal;
#endif
		XMFLOAT4 color;
		VertexType() {}
#ifndef FINAL
		VertexType(XMFLOAT3 p, XMFLOAT4 c) : position(p), color(c){}
#else
		VertexType(XMFLOAT3 p, XMFLOAT4 c, XMFLOAT3 n) : position(p), color(c), normal(n) {}
#endif
	};

public:
	ModelClass();
	ModelClass(float,float,float =0.0f,float = 0.0f,float = 0.0f,size_t = 0);
	ModelClass(const ModelClass&);
	~ModelClass();

	bool Initialize(ID3D11Device*);
	void Shutdown();
	void Render(ID3D11DeviceContext*);
	int GetIndexCount();

	void SetCurvLevel(size_t);
	void SetHeight(float);
	void SetRadius(float);
	void SetColor(float, float, float, float);
	void SetPosition(float, float, float);
private:
	bool InitializeBuffers(ID3D11Device*);
	void ShutdownBuffers();
	void RenderBuffers(ID3D11DeviceContext*);
	static XMFLOAT4 GenerateRandomColor();
	static XMFLOAT3 CalculateNormals(XMFLOAT3, vector<XMFLOAT3>&);
	static inline size_t CalculateIndexCount(const size_t);
	static inline size_t CalculateVertexCount(const size_t);
	void RefreshBuffers(ID3D11DeviceContext*);
private:
	ID3D11Buffer *m_vertexBuffer, *m_indexBuffer;
	bool m_hasChanged;
	XMFLOAT3 m_center;
	float m_height, m_radius;
	size_t  m_curvLevel;
	const size_t m_maxCurvLevel = 128;
	size_t m_vertexCount, m_indexCount;
	size_t m_maxVertexCount, m_maxIndexCount;
	XMFLOAT4 m_color;
};

#endif