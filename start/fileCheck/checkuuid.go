package fileCheck

import (
	"flag"
	"fmt"
	"io/ioutil"
	"os"
	"path/filepath"
	"strings"
	"time"
)

type Content struct {
	fileName string
	uuid string
}

var inputPath *string = flag.String("path", "../Assets", "要检查的路径")

func GetDir()  string {
	flag.Parse()
	dir,_:= filepath.Abs(*inputPath)
	return  dir
}

var baseDir string =GetDir()
var allFiles = make(map[string]*Content)

func GetAllFiles(root string)  {
	fileInfos ,err := ioutil.ReadDir(root)
	//fmt.Println(*inputPath)
	if err != nil {
		fmt.Println(root,err)
		os.Exit(1)
	}
	for _, file := range fileInfos  {
		fileName :=file.Name()
		path :=root+"/"+file.Name()
		relPath ,_:= filepath.Rel(baseDir,path)
		if !strings.HasPrefix(fileName,".")  {
			allFiles[relPath] = &Content{fileName:path}
			if file.IsDir() {
				GetAllFiles(path)
			}
		}
	}
}

func CheckMetaAndFiles()([]string ,[]string) {
	var filesArray [] string
	var metaArray [] string
	for k, v := range allFiles {
		if strings.HasSuffix(v.fileName,".meta") {
			newStr := strings.TrimSuffix(v.fileName,".meta")

			relPath ,_:= filepath.Rel(baseDir,newStr)
			if allFiles[relPath] == nil {
				filesArray = append(filesArray,k)
			}
			//fmt.Println(k,newStr,v.fileName)
			//fmt.Println(k,relPath)
		} else {
			//
			relPath ,_:= filepath.Rel(baseDir,v.fileName+".meta")
			if allFiles[relPath] == nil {
				metaArray = append(metaArray,k)
			}
		}
	}
	return  filesArray,metaArray
}

func Convert(maps []string ) string {
	msg := ""
	for _,v := range maps {
		msg += v
	}
	return  msg
}

func ThrowCheckFile()  {
	fileArray , metaArray := CheckMetaAndFiles()
	if Convert(fileArray) != "" {
		fmt.Println("文件缺少.meta文件",Convert(fileArray))
	}
	if Convert(metaArray) != "" {
		fmt.Println("文件缺少.meta文件",Convert(metaArray))
	}
}


func checkUUID()  {
	var startTime = time.Now()
	fmt.Println(baseDir)
	GetAllFiles(baseDir)
	ThrowCheckFile()
	var duration = time.Since(startTime)
	fmt.Println(fmt.Sprintf("执行完成! 耗时 %s",duration))
}

func init() {
	checkUUID()
}