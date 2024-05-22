all: build
SHELL:=/bin/bash
APP_NAME = "explore-cli"
.PHONY: dist

RIDS = "linux-x64" "linux-arm64" "linux-musl-x64" "linux-musl-arm64" "osx-x64" "osx-arm64" "win-x64" "win-arm64"

build: clean
	for rid in $(RIDS); do \
		echo "Building for $$rid..."; \
		dotnet publish -c Release -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=true -p:PublishTrimmed=true -p:StaticLink=true -r $$rid -o bin/$(APP_NAME)-$$rid; \
	done || true

# the above true condition is a yak shave, 
# /usr/local/share/dotnet/sdk/7.0.409/Sdks/Microsoft.NET.Sdk/targets/Microsoft.NET.Publish.targets(111,5): 
# error NETSDK1098: Applications published to a single-file are required to use the application host. 
# You must either set PublishSingleFile to false or set UseAppHost to true.

debug: clean
	@echo "Building in debug mode..."
	for rid in $(RIDS); do \
		echo "Building for $$rid..."; \
		dotnet publish -c Debug -p:PublishSingleFile=true -p:SelfContained=true -p:PublishReadyToRun=true-r $$rid -o bin/$(APP_NAME)-$$rid; \
	done

size:
	@echo "Size of the binaries:"
	for rid in $(RIDS); do \
		echo "Size of bin/$(APP_NAME)-$$rid:"; \
		ls -lh bin/$(APP_NAME)-$$rid/; \
	done

file:
	@echo "File type of the binaries:"
	for rid in $(RIDS); do \
		echo "File type of bin/$(APP_NAME)-$$rid:"; \
		file bin/$(APP_NAME)-$$rid/*; \
	done

dist:
	rm -rf dist
	mkdir -p dist
	for rid in $(RIDS); do \
		rm -f bin/$(APP_NAME)-$$rid/*.pdb; \
		cd bin/$(APP_NAME)-$$rid; \
		if [[ "$$rid" == *"win"* ]]; then \
			gzip --stdout --best *.Cli* > $(APP_NAME)-$$rid.exe.gz; \
			shasum -a 256 $(APP_NAME)-$$rid.exe.gz > $(APP_NAME)-$$rid.exe.gz.256; \
			mv $(APP_NAME)-$$rid.exe.gz ../../dist;\
			mv $(APP_NAME)-$$rid.exe.gz.256 ../../dist;\
		else \
			gzip --stdout --best *.Cli* > $(APP_NAME)-$$rid.gz; \
			shasum -a 256 $(APP_NAME)-$$rid.gz > $(APP_NAME)-$$rid.gz.256; \
			mv $(APP_NAME)-$$rid.gz ../../dist;\
			mv $(APP_NAME)-$$rid.gz.256 ../../dist;\
		fi; \
		cd ../../; \
	done

clean:
	@echo "Cleaning..."
	@rm -rf bin
	@rm -rf obj
	@rm -rf dist