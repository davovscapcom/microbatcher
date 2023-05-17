# MicroBatcher

This micro-batching library is an exercise for a job interview. It's
intent is to provide a way of micro-batching *jobs* and submitting them
for processing at a specified interval.

## Setup
This Microbatcher requires the .NET 7 SDK to build. Once built, reference the
dll in your project to use the Microbatcher.

When creating a new instance of Microbatcher, you must provide an ILogger, and an
IBatchProcessor to the constructor.

MicroBatcher can be configured to send batches when a certain Job threshold
is reached, when a time interval has elapsed, or both. To set these values,
you must set the following environment variables:

| Variable Name				  | Default Value |
|-----------------------------|---------------|
| MICROBATCHER__MAXBATCHSIZE  | 32            |
| MICROBATCHER__BATCHINTERVAL | 20_0000       |


## Usage
Submit a Job using SubmitJob(Job). Your job will be processed at the next
configured interval.

Prevent the submission of future jobs calling calling Shutdown(). Once all
remaining jobs have been processed, Shutdown() will return.
