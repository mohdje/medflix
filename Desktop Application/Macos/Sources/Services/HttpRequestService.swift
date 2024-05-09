import Foundation

class HttpRequestService {
    func fetchObject<T: Decodable>(url: String, completion: @escaping (Result<T, Error>) -> Void, headers: Dictionary<String, String>? = nil) {
        guard let url = URL(string: url) else {
            completion(.failure(NSError(domain: "", code: -1, userInfo: nil)))
            return
        }
        
        var urlRequest = URLRequest(url: url)
        
        if let requestHeaders = headers {
            for (headerKey, headerValue) in requestHeaders {
                urlRequest.setValue(headerValue, forHTTPHeaderField: headerKey)
            }
        }
        
        URLSession.shared.dataTask(with: urlRequest) { data, response, err in
            if let err = err {
                completion(.failure(err))
                return
            }
            
            guard let response = response as? HTTPURLResponse else { return }
            
            if (response.statusCode == 200 ){
                guard let data = data else {
                    completion(.failure(NSError(domain: "", code: -1, userInfo: nil)))
                    return
                }
                
                do {
                    let decoder = JSONDecoder()
                    let objectResponse = try decoder.decode(T.self, from: data)
                    completion(.success(objectResponse))
                } catch {
                    completion(.failure(error))
                }
            }
            else {
                completion(.failure(NSError(domain: "", code: response.statusCode, userInfo: nil)))
            }
        }.resume()
    }
    
    func ping(url: String, completion: @escaping (Result<Bool, Error>) -> Void) {
        guard let url = URL(string: url) else {
            completion(.failure(NSError(domain: "", code: -1, userInfo: nil)))
            return
        }
        
        let urlRequest = URLRequest(url: url)
        
        URLSession.shared.dataTask(with: urlRequest) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }
            
            guard let response = response as? HTTPURLResponse else { return }
            if (response.statusCode == 200 ){
                completion(.success(true))
            }
            else {
                completion(.failure(NSError(domain: "", code: response.statusCode, userInfo: nil)))
            }
        }.resume()
    }
    
    func putRequest(url: String, jsonData: Data, completion: @escaping (Result<Bool, Error>) -> Void) {
        guard let url = URL(string: url) else {
            completion(.failure(NSError(domain: "", code: -1, userInfo: nil)))
            return
        }
        
        var urlRequest = URLRequest(url: url)
        urlRequest.httpMethod = "PUT"
        urlRequest.httpBody = jsonData
        urlRequest.setValue("application/json", forHTTPHeaderField: "Content-Type")
        
        URLSession.shared.dataTask(with: urlRequest) { data, response, error in
            if let error = error {
                completion(.failure(error))
                return
            }
            
            guard let response = response as? HTTPURLResponse else { return }
            
            if (response.statusCode == 200 ){
                completion(.success(true))
            }
            else {
                completion(.failure(NSError(domain: "", code: response.statusCode, userInfo: nil)))
            }
        }.resume()
    }
    
    func downloadFile(fileUrl: String, destinationFileName: String, headers: Dictionary<String, String>? = nil, completion: @escaping (Bool, URL?) -> Void) {
        let fileURL = URL(string: fileUrl)
        
        let sessionConfig = URLSessionConfiguration.default
        let session = URLSession(configuration: sessionConfig)
        
        var request = URLRequest(url: fileURL!)
        if let requestHeaders = headers {
            for (headerKey, headerValue) in requestHeaders {
                request.setValue(headerValue, forHTTPHeaderField: headerKey)
            }
        }
        
        let downloadedFileUrl = FileManager.default.urls(for: .downloadsDirectory, in: .userDomainMask).first?.appendingPathComponent(destinationFileName)
        
        let task = session.downloadTask(with: request) { (tempLocalUrl, response, error) in
            if let tempLocalUrl = tempLocalUrl, error == nil {
                if let statusCode = (response as? HTTPURLResponse)?.statusCode {
                    if statusCode == 200 {
                        let fileManager = FileManager.default
                        
                        do {
                            if fileManager.fileExists(atPath: downloadedFileUrl!.path) {
                                try fileManager.removeItem(atPath: downloadedFileUrl!.path)
                            }
                            try fileManager.copyItem(at: tempLocalUrl, to: downloadedFileUrl!)
                            completion(true, downloadedFileUrl)
                            return
                        } catch {
                            completion(false, URL(string:""))
                        }
                    }
                }
            }
            completion(false, URL(string:""))
        }
        
        task.resume()
    }
}


