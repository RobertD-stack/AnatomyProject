# Master RPC Example

This example shows how to use slave to master RPCs. This kind of RPC can be useful when a task
takes a different time on each node, and we want to the master to know when all nodes are done.
For instance, asynchronously loading a file from a drive.
