# BlazorElectronics
Full-stack mock e-commerce web application made in asp.net with Blazor WASM in .net 7
Succesfully hosted app & databse on Azure. Can activate upon request to see (offline for now to save money)

Features:
  - client-side c#; minimal javascript
  - database first approach with dapper querying & sql
  - controller > service > repository server architecture
  - thin controllers
  - all logic made as asynchrounous as possible for performance
  - compressed dtos that can be used to regenerate more complex data on client; reduces netowrk latency/data usage
  - admin CRUD featured for categories, features, specifications, and vendors
  - admin product, user, and review generators; will insert coherent, but random rows
  - admin product edit is integrated seamlessly with normal product search
  - variable app functionality based on user login type (customer/admin)
  - user account registration with email verification
  - server-side user sessions with session tokens
  - comprehensive product search/filterting
  - multi-level product categories
  - mix of lookup & raw xml product specifications; to simulate external vendors providing their own specs, but can still filter by the lookups
  - local/server shopping cart
  - credit card checkout with Stripe
  - self-invalidating server caches; uses a timer to periodically ping the db for any changes, and updates cache

ToDo:
  - add ablilty for user to wrtie product reviews
  - finish integrating promo codes with checkout
  - set product keys to purchased/used on order fulfillment
