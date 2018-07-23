	////////////////////////////////////////////////////////////////////////////////
// Filename: modelclass.cpp
////////////////////////////////////////////////////////////////////////////////
#include "stdafx.h"
#include "modelclass.h"
#include <algorithm>


ModelClass::ModelClass()
{
	m_vertexBuffer = 0;
	m_indexBuffer = 0;
	m_curvLevel = 4;
	m_maxVertexCount = 128;
	m_maxIndexCount = CalculateIndexCount(m_maxCurvLevel);
	m_color = GenerateRandomColor();
}

ModelClass::ModelClass(float radius, float height,float x,float y,float z, size_t curvLevel)
{
	if (curvLevel + 4 > m_maxCurvLevel) curvLevel = m_maxCurvLevel - 4;
	m_vertexBuffer = 0;
	m_indexBuffer = 0;
	m_curvLevel = curvLevel+4;
	m_maxVertexCount = m_maxCurvLevel*2;
	m_maxIndexCount = CalculateIndexCount(m_maxCurvLevel);
	m_radius = radius;
	m_height = height;
	m_color = GenerateRandomColor();
	m_center = XMFLOAT3(x, y, z);
}


ModelClass::ModelClass(const ModelClass& other)
{
}


ModelClass::~ModelClass()
{
}


bool ModelClass::Initialize(ID3D11Device* device)
{
	bool result;


	// Initialize the vertex and index buffers.
	result = InitializeBuffers(device);
	if(!result)
	{
		return false;
	}

	return true;
}


void ModelClass::Shutdown()
{
	// Shutdown the vertex and index buffers.
	ShutdownBuffers();

	return;
}


void ModelClass::Render(ID3D11DeviceContext* deviceContext)
{
	// Put the vertex and index buffers on the graphics pipeline to prepare them for drawing.
	RenderBuffers(deviceContext);

	return;
}

int ModelClass::GetIndexCount()
{
	return m_indexCount;
}

void ModelClass::SetCurvLevel(size_t cl)
{
	if ((cl+4) > m_maxCurvLevel) cl = m_maxCurvLevel+4;
	m_hasChanged = cl != (m_curvLevel-4);
	m_curvLevel = cl+4;
}

void ModelClass::SetHeight(float height)
{
	m_hasChanged = (height != m_height);
	height = m_height;
}

void ModelClass::SetRadius(float radius)
{
	m_hasChanged = radius != m_radius;
	m_radius = radius;
}

void ModelClass::SetColor(float red, float green, float blue, float alpha)
{
	m_color = XMFLOAT4(red,green,blue,alpha);
}

void ModelClass::SetPosition(float x, float y, float z)
{
	m_hasChanged = m_center.x != x || m_center.y != y || m_center.z != z;
	m_center = XMFLOAT3(x, y, z);
}


bool ModelClass::InitializeBuffers(ID3D11Device* device)
{
	VertexType* vertices;
	unsigned long* indices;
	D3D11_BUFFER_DESC vertexBufferDesc, indexBufferDesc;
    D3D11_SUBRESOURCE_DATA vertexData, indexData;
	HRESULT result;


	// Set the number of vertices in the vertex array.
	m_vertexCount = CalculateVertexCount(m_curvLevel);

	// Set the number of indices in the index array.
	m_indexCount = CalculateIndexCount(m_curvLevel);

	// Create the vertex array.
	vertices = new VertexType[m_maxVertexCount]();
	float angleTurn = 2 * XM_PI / m_curvLevel;
	for (int i = 0; i < m_curvLevel; i++) {
		vertices[2 * i] = { 
			{m_center.x + m_radius * cos(angleTurn*i),m_center.y - m_height / 2,m_center.z + m_radius * sin(i*angleTurn)}, m_color
		};
		vertices[2 * i+1] = { 
			{m_center.x + m_radius * cos(angleTurn*i),m_center.y + m_height / 2,m_center.z + m_radius * sin(i*angleTurn)}, m_color
		};
	}
	vertices[m_vertexCount - 2] = { {m_center.x,m_center.y + m_height / 2,m_center.z},m_color };
	vertices[m_vertexCount - 1] = { {m_center.x, m_center.y - m_height / 2, m_center.z}, m_color };
	indices = new unsigned long[m_maxIndexCount];
	unsigned long indx = 0;
	for (int i = 0; i < m_curvLevel; i++) {
		indices[indx++] = 2 * ((i + 1) % m_curvLevel);
		indices[indx++] = 2 * i;
		indices[indx++] = 2 * ((i + 1) % m_curvLevel) + 1;
		indices[indx++] = 2 * ((i + 1) % m_curvLevel) + 1;
		indices[indx++] = 2 * i;
		indices[indx++] = 2 * i + 1;
	}
	for (int i = 0; i < m_curvLevel; i++) {
		indices[indx++] = 2* i +1;
		indices[indx++] = m_vertexCount - 2;
		indices[indx++] = 2 * ((i+1) % m_curvLevel) + 1;
	}
	for(int i = m_curvLevel-1; i >= 0; i--){
		indices[indx++] = 2 * ((i+1) % m_curvLevel) ;
		indices[indx++] = m_vertexCount - 1;
		indices[indx++] = 2 * ((i + m_curvLevel) % m_curvLevel);
	}
	// Set up the description of the static vertex buffer.
    vertexBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    vertexBufferDesc.ByteWidth = sizeof(VertexType) * m_maxVertexCount;
    vertexBufferDesc.BindFlags = D3D11_BIND_VERTEX_BUFFER;
    vertexBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    vertexBufferDesc.MiscFlags = 0;
	vertexBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the vertex data.
    vertexData.pSysMem = vertices;
	vertexData.SysMemPitch = 0;
	vertexData.SysMemSlicePitch = 0;

	// Now create the vertex buffer.
    result = device->CreateBuffer(&vertexBufferDesc, &vertexData, &m_vertexBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Set up the description of the static index buffer.
    indexBufferDesc.Usage = D3D11_USAGE_DYNAMIC;
    indexBufferDesc.ByteWidth = sizeof(unsigned long) * m_maxIndexCount;
    indexBufferDesc.BindFlags = D3D11_BIND_INDEX_BUFFER;
    indexBufferDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
    indexBufferDesc.MiscFlags = 0;
	indexBufferDesc.StructureByteStride = 0;

	// Give the subresource structure a pointer to the index data.
    indexData.pSysMem = indices;
	indexData.SysMemPitch = 0;
	indexData.SysMemSlicePitch = 0;

	// Create the index buffer.
	result = device->CreateBuffer(&indexBufferDesc, &indexData, &m_indexBuffer);
	if(FAILED(result))
	{
		return false;
	}

	// Release the arrays now that the vertex and index buffers have been created and loaded.
	delete [] vertices;
	vertices = 0;

	delete [] indices;
	indices = 0;

	return true;
}


void ModelClass::ShutdownBuffers()
{
	// Release the index buffer.
	if(m_indexBuffer)
	{
		m_indexBuffer->Release();
		m_indexBuffer = 0;
	}

	// Release the vertex buffer.
	if(m_vertexBuffer)
	{
		m_vertexBuffer->Release();
		m_vertexBuffer = 0;
	}

	return;
}


void ModelClass::RenderBuffers(ID3D11DeviceContext* deviceContext)
{
	unsigned int stride;
	unsigned int offset;

	if (m_hasChanged) {
		RefreshBuffers(deviceContext);
		m_hasChanged = false;
	}
	// Set vertex buffer stride and offset.
	stride = sizeof(VertexType); 
	offset = 0;
    
	// Set the vertex buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetVertexBuffers(0, 1, &m_vertexBuffer, &stride, &offset);

    // Set the index buffer to active in the input assembler so it can be rendered.
	deviceContext->IASetIndexBuffer(m_indexBuffer, DXGI_FORMAT_R32_UINT, 0);

    // Set the type of primitive that should be rendered from this vertex buffer, in this case triangles.
	deviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

	return;
}
XMFLOAT4 ModelClass::GenerateRandomColor() {
	return XMFLOAT4(rand() % 100 / 100.0f, rand() % 100 / 100.0f, rand() % 100 / 100.0f, rand() % 100 / 100.0f);
}

XMFLOAT3 ModelClass::CalculateNormals(XMFLOAT3 center, vector<XMFLOAT3>& points)
{
	XMVECTOR vecResult;
	XMVECTOR distVec1,distVec2;
	XMFLOAT3 distCoor;
	for (int i = 0; i < points.size(); i++) {
		distCoor = { points[i].x - center.x,points[i].y - center.y, points[i].z - center.z };
		distVec1 = XMLoadFloat3(&distCoor);
		distCoor = { points[(i + 1) % points.size()].x - center.x,points[(i + 1) % points.size()].y - center.y,
			points[(i + 1) % points.size()].z - center.z };
		distVec2 = XMLoadFloat3(&distCoor);
		vecResult += XMVector3Cross(distVec1, distVec2);
	}
	vecResult = XMVector3Normalize(vecResult);
	XMFLOAT3 pointResult;
	XMStoreFloat3(&pointResult, vecResult);
	return pointResult;
}

size_t ModelClass::CalculateIndexCount(const size_t curvLevel)
{
	// 6 indices for side surface and 3 indices for each up and down sides 
	return 6* curvLevel +  6* curvLevel;
}

size_t ModelClass::CalculateVertexCount(const size_t curvLevel)
{
	return 2*curvLevel + 2;
}

void ModelClass::RefreshBuffers(ID3D11DeviceContext* deviceContext)
{
	m_vertexCount = CalculateVertexCount(m_curvLevel);

	// Set the number of indices in the index array.
	m_indexCount = CalculateIndexCount(m_curvLevel);

	// Create the vertex array.
	auto vertices = new VertexType[m_maxVertexCount]();
	float angleTurn = 2 * XM_PI / m_curvLevel;
	for (int i = 0; i < m_curvLevel; i++) {
		vertices[2 * i] = {
			{ m_center.x + m_radius * cos(angleTurn*i),m_center.y - m_height / 2,m_center.z + m_radius * sin(i*angleTurn) }, m_color
		};
		vertices[2 * i + 1] = {
			{ m_center.x + m_radius * cos(angleTurn*i),m_center.y + m_height / 2,m_center.z + m_radius * sin(i*angleTurn) }, m_color
		};
	}
	vertices[m_vertexCount - 2] = { {m_center.x,m_center.y + m_height / 2,m_center.z},m_color };
	vertices[m_vertexCount - 1] = { {m_center.x, m_center.y - m_height / 2, m_center.z}, m_color };
	auto indices = new unsigned long[m_maxIndexCount];
	unsigned long indx = 0;
	for (int i = 0; i < m_curvLevel; i++) {
		indices[indx++] = 2 * ((i + 1) % m_curvLevel);
		indices[indx++] = 2 * i;
		indices[indx++] = 2 * ((i + 1) % m_curvLevel) + 1;
		indices[indx++] = 2 * ((i + 1) % m_curvLevel) + 1;
		indices[indx++] = 2 * i;
		indices[indx++] = 2 * i + 1;
	}
	for (int i = 0; i < m_curvLevel; i++) {
		indices[indx++] = 2* i + 1;
		indices[indx++] = m_vertexCount - 2;
		indices[indx++] = 2 * ((i+1) % m_curvLevel) + 1;
	}
	for(int i = m_curvLevel-1; i >= 0; i--){
		indices[indx++] = 2 * ((i+1) % m_curvLevel);
		indices[indx++] = m_vertexCount - 1;
		indices[indx++] = 2* ((i+m_curvLevel)% m_curvLevel);
	}
	D3D11_MAPPED_SUBRESOURCE mappedResource;
	deviceContext->Map(m_vertexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
	//mappedResource.pData = (void*) new VertexType[m_vertexCount];
	memcpy(mappedResource.pData, vertices, sizeof(VertexType)*m_vertexCount);
	deviceContext->Unmap(m_vertexBuffer, 0);
	deviceContext->Map(m_indexBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource);
	memcpy(mappedResource.pData, indices, sizeof(unsigned long)*m_indexCount);
	deviceContext->Unmap(m_indexBuffer, 0);
	delete[] vertices;
	delete[] indices;
}
