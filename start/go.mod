module start

go 1.12

require gopkg.in/yaml.v2 v2.2.2

replace gopkg.in/yaml.v2 v2.2.2 => ./src/yaml

replace fileCheck => ./src/fileCheck
