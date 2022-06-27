FROM sdflysha/dotnet6-opencv:4.6.0-ubuntu20 as builder

WORKDIR /

# Install wget dependencies
RUN apt-get update && apt-get -y install --no-install-recommends wget \
    && apt-get -y clean \
    && rm -rf /var/lib/apt/lists/*

RUN wget -q https://paddle-inference-lib.bj.bcebos.com/2.3.0/cxx_c/Linux/CPU/gcc8.2_avx_mkl/paddle_inference_c.tgz && \
    tar -xzf /paddle_inference_c.tgz && \
    find /paddle_inference_c -mindepth 2 -name *.so* -print0 | xargs -0 -I {} mv {} /usr/lib && \
    ls /usr/lib/*.so* && \
    rm -rf /paddle_inference_c && \
    rm paddle_inference_c.tgz

########## Final image ##########

FROM sdflysha/dotnet6-opencv:4.6.0-ubuntu20 as final
COPY --from=builder /usr/lib /usr/lib