# BlazorElectronics
Portfolio Ecommerce Website. This is a mock product-key site. Essentially, this would be a site that sells the keys to digital products provided by various vendors like Amazon, Steam, etc.

Made in .Net7, using Blazor WASM

Client uses c# compiled to web assembly.

Server project designed to be independant of client, meaning its like like a normal Asp.Net web API.

Server side mostly done. 

Server architecture is Controller > Services > DbRepositories.

I only use Dapper for db interaction, no EF Core.

Specifications and Categories are cached locally in server memory instead of using a distributed cache. This save the network lag and json deserialization. These cached objects are self-invalidating, and I plan to add web hooks to update the data on admin change.

User sessions are stored on the server, no JWT or other client side validation. All client api requests go to server. But these sessions will be cached in memory as well.

The client side is quite unfinished, but takes very little time to complete due to the ability to use pure c#, and share the DTOs.

TODO:
- Finish admin functionality.
- Add payment with Stripe checkout.
- Add delivery address for physical products.
- Finish client side.
- Upload DB Schema and Stored Procedures.
