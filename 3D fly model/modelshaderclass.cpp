////////////////////////////////////////////////////////////////////////////////
// Filename: colorshaderclass.cpp
////////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "modelshaderclass.h"


ModelShaderClass::ModelShaderClass()
{
	m_vertexShader = 0;
	m_pixelShader = 0;
	m_layout = 0;
	m_matrixBuffer = 0;
#ifdef FINAL
	m_lightBuffer = 0;
#endif
}


ModelShaderClass::ModelShaderClass(const ModelShaderClass& other)
{
}


ModelShaderClass::~ModelShaderClass()
{
}


bool ModelShaderClass::Initialize(ID3D11Device* device, HWND hwnd)
{
	bool result;


	// Initialize the vertex and pixel shaders.
#ifndef FINAL
	result = InitializeShader(device, hwnd, L"shaders/track.vs", L"shaders/track.ps");
#else
	result = InitializeShader(device, hwnd, L"shaders/model.vs", L"shaders/model.ps");
#endif
	if(!result)
	{
		return false;
	}

	return true;
}


void ModelShaderClass::Shutdown()
{
	// Shutdown the vertex and pixel shaders as well as the related objects.
	ShutdownShader();

	return;
}

//#ifndef FINAL
bool ModelShaderClass::Render(ID3D11DeviceContext* deviceContext, int indexCount, XMMATRIX worldMatrix, XMMATRIX viewMatrix,
							  XMMATRIX projectionMatrix)
//#else
//bool ModelShaderClass::Render(ID3D11DeviceContext* deviceContext, int indexCount, XMMATRIX worldMatrix, XMMATRIX viewMatrix,
//							  XMMATRIX projectionMatrix,XMFLOAT4 ambientColor, XMFLOAT4 diffuseColor, XMFLOAT3 lightDirection)
//#endif
{
	bool result;


	// Set the shader parameters that it will use for rendering.
//#ifndef FINAL
	result = SetShaderParameters(deviceContext, worldMatrix, viewMatrix, projectionMatrix);
//#else
//	result = SetShaderParameters(deviceContext,worldMatrix,viewMatrix,projectionMatrix,ambientColor,diffuseColor,lightDirection)
//#endif
	if(!result)
	{
		return false;
	}

	// Now render the prepared buffers with the shader.
	RenderShader(deviceContext, indexCount);

	return true;
}


bool ModelShaderClass::InitializeShader(ID3D11Device* device, HWND hwnd,const WCHAR* vsFilename,const WCHAR* psFilename)
{
	HRESULT result;
	ID3D10Blob* errorMessage;
	ID3D10Blob* vertexShaderBuffer;
	ID3D10Blob* pixelShaderBuffer;
	unsigned int numElements;
	D3D11_BUFFER_DESC matrixBufferDesc;


	// Initialize the pointers this function will use to null.
	errorMessage = 0;
	vertexShaderBuffer = 0;
	pixelShaderBuffer = 0;

    // Compile the vertex shader code.
#ifndef FINAL
	result = D3DCompileFromFile(vsFilename, NULL, NULL, "TrackVertexShader", "vs_5_0", D3D10_SHADER_ENABLE_STRICTNESS, 0,
								&vertexShaderBuffer, &errorMessage);
#else
	result = D3DCompileFromFile(vsFilename, NULL, NULL, "ModelVertexShader", "vs_5_0", D3D10_SHADER_ENABLE_STRICTNESS, 0,
		&vertexShaderBuffer, &errorMessage);
#endif
	if(FAILED(result))
	{
		// If the shader failed to compile it should have writen something to the error message.
		if(errorMessage)
		{
			OutputShaderErrorMessage(errorMessage, hwnd, vsFilename);
		}
		// If there was  nothing in the error message then it simply could not find the shader file itself.
		else
		{
			MessageBox(hwnd, vsFilename, L"Missing Shader File", MB_OK);
		}

		return false;
	}

    // Compile the pixel shader code.
#ifndef FINAL
	result = D3DCompileFromFile(psFilename, NULL, NULL, "TrackPixelShader", "ps_5_0", D3D10_SHADER_ENABLE_STRICTNESS, 0,
								&pixelShaderBuffer, &errorMessage);
#else 
	result = D3DCompileFromFile(psFilename, NULL, NULL, "ModelPixelShader", "ps_5_0", D3D10_SHADER_ENABLE_STRICTNESS, 0,
		&pixelShaderBuffer, &errorMessage);
#endif
	if(FAILED(result))
	{
		// If the shader failed to compile it should have writen something to the error message.
		if(errorMessage)
		{
			OutputShaderErrorMessage(errorMessage, hwnd, psFilename);
		}
		// If there was nothing in the error message then it simply could not find the file itself.
		else
		{
			MessageBox(hwnd, psFilename, L"Missing Shader File", MB_OK);
		}

		return false;
	}

    // Create the vertex shader from the buffer.
    result = device->CreateVertexShader(vertexShaderBuffer->GetBufferPointer(), vertexShaderBuffer->GetBufferSize(),
		NULL, &m_vertexShader);
	if(FAILED(result))
	{
		return false;
	}

    // Create the pixel shader from the buffer.
    result = device->CreatePixelShader(pixelShaderBuffer->GetBufferPointer(), pixelShaderBuffer->GetBufferSize(),
		NULL, &m_pixelShader);
	if(FAILED(result))
	{
		return false;
	}
	D3D11_INPUT_ELEMENT_DESC polygonLayout[] = { 
		{"POSITION",0,DXGI_FORMAT_R32G32B32_FLOAT,0,0,D3D11_INPUT_PER_VERTEX_DATA},
#ifdef FINAL
	{ "NORMAL",0, DXGI_FORMAT_R32G32B32_FLOAT,0,D3D11_APPEND_ALIGNED_ELEMENT,D3D11_INPUT_PER_VERTEX_DATA,0 },
#endif
		{"COLOR", 0 ,DXGI_FORMAT_R32G32B32A32_FLOAT,0,D3D11_APPEND_ALIGNED_ELEMENT,D3D11_INPUT_PER_VERTEX_DATA,0},
	};

	// Get a count of the elements in the layout.
    numElements = sizeof(polygonLayout) / sizeof(polygonLayout[0]);

	// Create the vertex input layout.
	result = device->CreateInputLayout(polygonLayout, numElements, vertexShaderBuffer->GetBufferPointer(), 
									   vertexShaderBuffer->GetBufferSize(), &m_layout);
	if(FAILED(result))
	{
		return false;
	}

	// Release the vertex shader buffer and pixel shader buffer since they are no longer needed.
	vertexShaderBuffer->Release();
	vertexShaderBuffer = 0;

	pixelShaderBuffer->Release();
	pixelShaderBuffer = 0;

    // Setup the description of the dynamic matrix constant buffer that is in the vertex shader.
    matrixBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
	matrixBufferDesc.ByteWidth = sizeof(MatrixBufferType);
    matrixBufferDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
    matrixBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    matrixBufferDesc.MiscFlags = 0;
	matrixBufferDesc.StructureByteStride = 0;

	// Create the constant buffer pointer so we can access the vertex shader constant buffer from within this class.
	result = device->CreateBuffer(&matrixBufferDesc, NULL, &m_matrixBuffer);
	if(FAILED(result))
	{
		return false;
	}
#ifdef FINAL
	D3D11_BUFFER_DESC lightBufferDesc = { sizeof(LightBufferType),D3D11_USAGE_DYNAMIC,D3D11_BIND_CONSTANT_BUFFER,
		D3D11_CPU_ACCESS_WRITE,0,0 };
	result = device->CreateBuffer(&lightBufferDesc, NULL, &m_lightBuffer);
	if (FAILED(result)) {
		return false;
	}
#endif
	return true;
}


void ModelShaderClass::ShutdownShader()
{
	// Release the matrix constant buffer.
	if(m_matrixBuffer)
	{
		m_matrixBuffer->Release();
		m_matrixBuffer = 0;
	}

	// Release the layout.
	if(m_layout)
	{
		m_layout->Release();
		m_layout = 0;
	}

	// Release the pixel shader.
	if(m_pixelShader)
	{
		m_pixelShader->Release();
		m_pixelShader = 0;
	}

	// Release the vertex shader.
	if(m_vertexShader)
	{
		m_vertexShader->Release();
		m_vertexShader = 0;
	}
#ifdef FINAL
	if (m_lightBuffer) {
		m_lightBuffer->Release();
		m_lightBuffer = 0;
	}
#endif
	return;
}


void ModelShaderClass::OutputShaderErrorMessage(ID3D10Blob* errorMessage, HWND hwnd,const WCHAR* shaderFilename)
{
	char* compileErrors;
	unsigned long long bufferSize, i;
	ofstream fout;


	// Get a pointer to the error message text buffer.
	compileErrors = (char*)(errorMessage->GetBufferPointer());

	// Get the length of the message.
	bufferSize = errorMessage->GetBufferSize();

	// Open a file to write the error message to.
	fout.open("shader-error.txt");

	// Write out the error message.
	for(i=0; i<bufferSize; i++)
	{
		fout << compileErrors[i];
	}

	// Close the file.
	fout.close();

	// Release the error message.
	errorMessage->Release();
	errorMessage = 0;

	// Pop a message up on the screen to notify the user to check the text file for compile errors.
	MessageBox(hwnd, L"Error compiling shader.  Check shader-error.txt for message.", shaderFilename, MB_OK);

	return;
}
//#ifndef FINAL
bool ModelShaderClass::SetShaderParameters(ID3D11DeviceContext * deviceContext, XMMATRIX worldMatrix, XMMATRIX viewMatrix, XMMATRIX projectionMatrix)
//#else
//bool ModelShaderClass::SetShaderParameters(ID3D11DeviceContext* deviceContext, XMMATRIX worldMatrix, XMMATRIX viewMatrix,
//		XMMATRIX projectionMatrix, XMFLOAT4 ambientColor, XMFLOAT4 diffuseColor, XMFLOAT3 direction)
//#endif
{
	HRESULT result;
    D3D11_MAPPED_SUBRESOURCE mappedResource;
	MatrixBufferType* dataPtr;
	unsigned int bufferNumber;


	// Transpose the matrices to prepare them for the shader.
	worldMatrix = XMMatrixTranspose(worldMatrix);
	viewMatrix = XMMatrixTranspose(viewMatrix);
	projectionMatrix = XMMatrixTranspose(projectionMatrix);

	// Lock the constant buffer so it can be written to.
	result = deviceContext->Map(m_matrixBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
	if(FAILED(result))
	{
		return false;
	}

	// Get a pointer to the data in the constant buffer.
	dataPtr = (MatrixBufferType*)mappedResource.pData;

	// Copy the matrices into the constant buffer.
	dataPtr->world = worldMatrix;
	dataPtr->view = viewMatrix;
	dataPtr->projection = projectionMatrix;

	// Unlock the constant buffer.
    deviceContext->Unmap(m_matrixBuffer, 0);

	// Set the position of the constant buffer in the vertex shader.
	bufferNumber = 0;

	// Finanly set the constant buffer in the vertex shader with the updated values.
    deviceContext->VSSetConstantBuffers(bufferNumber, 1, &m_matrixBuffer);
#ifdef FINAL
	result = deviceContext->Map(m_lightBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
	if (FAILED(result)) return false;
	auto dataPtr = (LightBufferType*)mappedResource.pData;
	dataPtr->diffuseColor = diffuseColor;
	dataPtr->ambientColor = ambientColor;
	dataPtr->direction = lightDirection;
	dataPtr->padding = 0.0f;
	deviceContext->Unmap(m_lightBuffer, 0);
	deviceContext->PSSetConstantBuffers(0, 1, &m_lightBuffer);
#endif
	return true;
}


void ModelShaderClass::RenderShader(ID3D11DeviceContext* deviceContext, int indexCount)
{
	// Set the vertex input layout.
	deviceContext->IASetInputLayout(m_layout);

    // Set the vertex and pixel shaders that will be used to render this triangle.
    deviceContext->VSSetShader(m_vertexShader, NULL, 0);
    deviceContext->PSSetShader(m_pixelShader, NULL, 0);

	// Render the triangle.
	deviceContext->DrawIndexed(indexCount, 0, 0);

	return;
}