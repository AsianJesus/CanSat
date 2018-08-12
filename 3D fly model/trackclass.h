////////////////////////////////////////////////////////////////////////////////
// Filename: modelclass.h
////////////////////////////////////////////////////////////////////////////////
#ifndef _TRACKCLASS_H_
#define _TRACKCLASS_H_


//////////////
// INCLUDES //
//////////////
#include <d3d11.h>
#include <directxmath.h>
#include <vector>
using namespace DirectX;
using namespace std;

////////////////////////////////////////////////////////////////////////////////
// Class name: TrackClass
////////////////////////////////////////////////////////////////////////////////
class TrackClass
{
private:
	struct VertexType
	{
		XMFLOAT3 position;
		XMFLOAT4 color;
		VertexType() {}
		VertexType(XMFLOAT3 p, XMFLOAT4 c) : position(p), color(c){}
	};

public:
	TrackClass();
	TrackClass(size_t maxVertices, float r = 2.0f,float g = 2.0f,float b = 2.0f,float a = 2.0f);
	TrackClass(const TrackClass&);
	~TrackClass();

	bool Initialize(ID3D11Device*);
	void Shutdown();
	void Render(ID3D11DeviceContext*);
	void AddPoint(float, float, float);
	bool DeletePoint(float, float, float);
	void Clear();
	int GetIndexCount();

private:
	bool InitializeBuffers(ID3D11Device*);
	void ShutdownBuffers();
	void RenderBuffers(ID3D11DeviceContext*);
	static XMFLOAT4 GenerateRandomColor();
	void RefreshBuffers(ID3D11DeviceContext*);
private:
	ID3D11Buffer *m_vertexBuffer, *m_indexBuffer;
	XMFLOAT4* m_color;
	bool m_hasChanged;
	vector<XMFLOAT3> m_points;
	size_t m_vertexCount, m_indexCount;
	size_t m_maxVertexCount, m_maxIndexCount;
};

#endif