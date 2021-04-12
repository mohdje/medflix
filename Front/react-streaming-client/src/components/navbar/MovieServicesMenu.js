
import "../../style/movie-services-menu.css";

import fadeTransition from "../../js/customStyles.js";
import MoviesAPI from "../../js/moviesAPI.js";

import CircularProgressBar from "../common/CircularProgressBar";
import { useEffect, useState } from 'react';


function MovieServicesMenu({onClick, visible }) {
    
    const [services, setServices] = useState([]);
    const [dataLoading, setDataLoading] = useState(true);

    useEffect(()=>{
        if(visible){
            MoviesAPI.getAvailableMovieServices(
                (services) => {
                    setDataLoading(false);
                    if (services && services.length > 0)
                        setServices(services);
                },
                () => console.log('fail'));
        }
    },[visible]);

    const handleServiceClick= (serviceName)=>{
        setDataLoading(true);
        MoviesAPI.changeMovieService(serviceName,
            ()=> {window.location.reload()},
            () => console.log('fail'));
        onClick();
    }
    
    return (
        <div style={fadeTransition(visible)} className="movie-services-menu-container">    
            <div style={{display: dataLoading ?  '': 'none'}}>
                <CircularProgressBar color={'white'} size={'40px'} visible={true}/>      
            </div>          
           <div style={{display: !dataLoading ? '': 'none'}}>
               <div className="title">Movies Services</div>
                {services.map(service => (
                     <div 
                        key={service.name} 
                        className={"movie-service" + (service.selected ? " selected" : "")}
                        onClick={()=> handleServiceClick(service.name)}>
                        {service.name}
                    </div>)
               )}
           </div>
        </div>
    );
}
export default MovieServicesMenu;
