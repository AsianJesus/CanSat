////////////////////////////////////////////////////////////////////////////////
// Filename: colorshaderclass.h
////////////////////////////////////////////////////////////////////////////////
#ifndef _COLORSHADERCLASS_H_
#define _COLORSHADERCLASS_H_


//////////////
// INCLUDES //
//////////////
#include <d3d11.h>
#include <d3dcompiler.h>
#include <directxmath.h>
#include <fstream>
using namespace DirectX;
using namespace std;


////////////////////////////////////////////////////////////////////////////////
// Class name: ColorShaderClass
////////////////////////////////////////////////////////////////////////////////
class ModelShaderClass
{
private:
	struct MatrixBufferType
	{
		XMMATRIX world;
		XMMATRIX view;
		XMMATRIX projection;
	};
#ifdef FINAL
	struct LightBufferType {
		XMFLOAT4 ambientColor;
		XMFLOAT4 diffuseColor;
		XMFLOAT3 direction;
		float padding;
	};
#endif
public:
	ModelShaderClass();
	ModelShaderClass(const ModelShaderClass&);
	~ModelShaderClass();

	bool Initialize(ID3D11Device*, HWND);
	void Shutdown();
//#ifndef FINAL
	bool Render(ID3D11DeviceContext*, int, XMMATRIX, XMMATRIX, XMMATRIX);
//#else
//	bool Render(ID3D11DeviceContext*, int, XMMATRIX, XMMATRIX, XMMATRIX, XMFLOAT4, XMFLOAT4, XMFLOAT3);
//#endif
private:
	bool InitializeShader(ID3D11Device*, HWND,const WCHAR*,const WCHAR*);
	void ShutdownShader();
	void OutputShaderErrorMessage(ID3D10Blob*, HWND, const WCHAR*);
//#ifndef FINAL
	bool SetShaderParameters(ID3D11DeviceContext*, XMMATRIX, XMMATRIX, XMMATRIX);
//#else
//	bool SetShaderParameters(ID3D11DeviceContext*, XMMATRIX, XMMATRIX, XMMATRIX,XMFLOAT4,XMFLOAT4,XMFLOAT3);
//#endif	
	void RenderShader(ID3D11DeviceContext*, int);

private:
	ID3D11VertexShader* m_vertexShader;
	ID3D11PixelShader* m_pixelShader;
	ID3D11InputLayout* m_layout;
	ID3D11Buffer* m_matrixBuffer;
#ifdef FINAL
	ID3D11Buffer* m_lightBuffer;
#endif
};

#endif