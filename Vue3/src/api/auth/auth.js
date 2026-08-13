import request from "@/utils/request"

export function LoginApi(data) {
    return request.post("/Auth/Login", data)
}