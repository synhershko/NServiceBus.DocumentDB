NServiceBus.DocumentDB
======================

Persistence support for NServiceBus using the [Azure DocumentDB](http://azure.microsoft.com/en-us/services/documentdb/)

## Current status

For now this is just a Proof-of-Concept. It has implementation for Sagas, Timeouts and Subscriptions persisters, but none are exposed to NServiceBus as Features (which means they can't be picked up by NServiceBus).

Only the Subscriptions persistence is properly implemented and tested.

The Timeouts persister is broken because DateTime range queries are not supported by DocumentDB, neither they will be since ADB only supports querying on JSON primitives. As noted in the comment in TimeoutPersister, we should decide on the best way aroud that only once DocumentDB is out of preview.

The Sagas persister isn't fully implemented due the requirement of implementing Unique Constraints functionlaity, which isn't supported by ADB either. Same as before, we can decide on which route to take once ADB is out of preview and the feature set as well as its roadmap are more clear.
