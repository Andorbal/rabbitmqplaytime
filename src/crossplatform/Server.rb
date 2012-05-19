require 'rubygems'
require 'carrot'

q = Carrot.queue("tasks4", :durable => true, :exchange => "amq.direct")
q.publish("Hello world!")
q.publish("This works...")
q.publish("Well, bye!")
Carrot.stop