```mermaid
flowchart LR
	mvc[Controller-based API]
	minapi[Minimal API]
	defdescprov[DefaultApiDescriptionProvider]
	endpdescprov[EndpointMetadataDescriptionProvider]
	desccoll[IApiDescriptionCollection]
	compservice[[OpenApiComponentService]]
	docsvc[[OpenApiDocumentService]]
	idocprovider[[IDocumentProvider]]
	meas[Microsoft.Extensions.ApiDescription.Server]
	mvc --> defdescprov 
	mvc --> endpdescprov
	minapi --> endpdescprov
	defdescprov --> desccoll
	endpdescprov --> desccoll
	desccoll --> docsvc
	idocprovider --> docsvc
	meas --> idocprovider
	compservice --> docsvc
```